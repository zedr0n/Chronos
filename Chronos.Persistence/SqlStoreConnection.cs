﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Exceptions;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using Chronos.Persistence.Serialization;
using Chronos.Persistence.Types;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
//using Stream = Chronos.Persistence.Types.Stream;

namespace Chronos.Persistence
{
    public class SqlStoreConnection : IEventStoreConnection
    {
        private readonly IEventDb _eventDb;
        private readonly IEventSerializer _eventSerializer;
        private readonly ICommandSerializer _commandSerializer;
        private readonly ITimeline _timeline;

        private readonly Subject<Envelope> _events = new Subject<Envelope>();
        public IObservable<Envelope> Events => _events.AsObservable();

        private readonly IDebugLog _debugLog;

        public SqlStoreConnection(IEventDb eventDb, IEventSerializer eventSerializer, ITimeline timeline, IDebugLog debugLog, ICommandSerializer commandSerializer)
        {
            _eventDb = eventDb;
            _timeline = timeline;
            _debugLog = debugLog;
            _commandSerializer = commandSerializer;
            _eventSerializer = eventSerializer;
        }

        public void AppendCommand(ICommand command)
        {
            command.Timestamp = _timeline.Now();
            using (var context = _eventDb.GetContext())
            {
                var commandDto = _commandSerializer.Serialize(command);
                //_debugLog.WriteLine(commandDto.Payload);
                context.Set<Command>().Add(commandDto);
                context.SaveChanges();
            }
        }

        public IEnumerable<ICommand> ReadCommands(Instant from, Instant to, Guid timeline)
        {
            using (var context = _eventDb.GetContext())
            {
                var commands = context.Set<Command>().Where(c => c.TimestampUtc >= from.ToDateTimeUtc() &&
                                                                 c.TimestampUtc <= to.ToDateTimeUtc() &&
                                                                 c.Timeline == timeline);
                return commands.ToList().Select(_commandSerializer.Deserialize);
            }
        }

        private void TimestampEvents(IEnumerable<IEvent> events)
        {
            var timestamp = _timeline.Now(); 
            // we only update the timestamps where they haven't been preset
            foreach (var e in events.Where(e => e.Timestamp == default(Instant)))
                e.Timestamp = timestamp;
        }

        public IEnumerable<StreamDetails> GetStreams()
        {
            using (var context = _eventDb.GetContext())
            {
                var streams = context.Set<Stream>()
                    //.Where(s => s.TimelineId == _timeline.TimelineId)
                    .Select(s => new StreamDetails(s.Name)//new StreamDetails(s.SourceType,s.Key)
                {
                    SourceType = s.SourceType,
                    Version = s.Version,
                    Key = s.Key,
                    BranchVersion = s.BranchVersion,
                    Timeline = s.TimelineId
                }).OrderBy(x => x.IsBranch);

                return streams.ToList();
            }
        }

        private Stream OpenStreamForWriting(DbContext context, StreamDetails details) 
        {
            var streamName = details.Name;
            var version = GetStreamVersion(details);

            Stream stream;

            if (version == -1)
            {
                // create a new stream if none exists
                stream = new Stream
                {
                    HashId = streamName.HashString(),
                    Name = streamName,
                    SourceType = details.SourceType,
                    Key = details.Key,
                    Version = 0,
                    TimelineId = details.Timeline
                };
                context.Set<Stream>().Add(stream);

                if (details.Timeline == Guid.Empty)
                    return stream;
                
                // cloning the original stream up to current time
                var events = ReadStreamEventsForwardFromLiveTimeline(details,
                    -1, int.MaxValue).ToList();
                    
                if (!events.Any())
                {
                    // do not clone the stream if no events exist?
                    //stream.TimelineId = Guid.Empty;
                    return stream;
                }
                    
                stream.BranchVersion = events.Last().Version;
                details.BranchVersion = stream.BranchVersion;
                stream.Events.AddRange(events.Select(_eventSerializer.Serialize));
            }
            else
            {
                // create a dummy stream to avoid loading all events from database
                stream = new Stream
                {
                    HashId = streamName.HashString(),
                    Name = streamName,
                    SourceType = details.SourceType,
                    Key = details.Key,
                    Version = version,
                    TimelineId = details.Timeline
                };
                context.Set<Stream>().Attach(stream);

                if (details.Timeline == Guid.Empty)
                    return stream;
                
                // read new events from the original timeline if any from the original branch point
                var events = ReadStreamEventsForwardFromLiveTimeline(details,
                    GetStreamBranchVersion(details), int.MaxValue).ToList();
                   
                MergeStreams(stream,events);
            }
            
            return stream;
        }

        private void MergeStreams(Stream stream,IList<IEvent> events)
        {
            if (!events.Any())
                return;
            if(stream.Version != events.First().Version)
                throw new NotImplementedException(" Merging strategy for new events not implemented yet ");
            
            stream.Events.AddRange(events
                .Select(_eventSerializer.Serialize));
        }

        private void WriteStream(Stream stream, IEnumerable<IEvent> events)
        {
            var eventsDto = events.Select(_eventSerializer.Serialize).ToList();

            foreach (var e in eventsDto)
            {
                stream.Events.Add(e);
                // stream.Version will be ahead of aggregate version
                // at the end of the stream if a retroactive event is added
                // 
                stream.Version++;
                
                //Debug.WriteLine(stream.Name + " : " + e.Payload);
            }

        }

        private void LogEvents(StreamDetails stream,IList<IEvent> events)
        {
            if (stream.IsSaga)
                return;

            foreach (var e in events)
            {
                _debugLog.WriteLine(e.GetType().Name + "( " +
                                    InstantPattern.ExtendedIso.Format(e.Timestamp) + " )");
                _debugLog.WriteLine(_eventSerializer.Serialize(e).Payload);

            }
        }

        private bool Validate(IList<IEvent> events, IList<IEvent> futureEvents)
        {
            var futureEventIds = futureEvents.Select(e => e.Version);

            if (!events.Select(e => e.Version).SequenceEqual(futureEventIds))
            {
                if (_timeline.Live) // concurrency exception
                    return false;
            }
            // check if the events produced are different 
            if (!events.Select(e => e.Hash).SequenceEqual(futureEvents.Select(x => x.Hash)))
            {
                if (_timeline.Live)
                    return false;
                        
                throw new NotImplementedException("Modifying events not implemented yet");
            }
            return true;
        }

        public void AppendToStream(StreamDetails streamDetails, int expectedVersion, IEnumerable<IEvent> enumerable)
        {
            var events = enumerable as IList<IEvent> ?? enumerable.ToList();
            if (!events.Any())
                return;

            // we are always appending to current active timeline
            streamDetails.Timeline = _timeline.TimelineId;

            using (var context = _eventDb.GetContext())
            {
                var stream = OpenStreamForWriting(context, streamDetails);

                if (stream.Version < expectedVersion)
                    throw new ConcurrencyException("Stream version is not consistent with events : "
                                                         + "version : " + stream.Version + " < " + "expected :" + expectedVersion);
                if (stream.Version > expectedVersion)
                {
                    // check event numbers to see if we are trying to write a past event again
                    var futureEvents = ReadStreamEventsForward(streamDetails, expectedVersion,events.Count).ToList();
                    
                    if(!Validate(events,futureEvents))
                        throw new InvalidOperationException("Trying to modify past for stream " + stream.Name + 
                                                            " : " + stream.Version + " > " + expectedVersion);    
                    
                    // otherwise we do not need to write an event again
                    return;
                }

                TimestampEvents(events);
                WriteStream(stream, events);
                
                streamDetails.Version = stream.Version; //expectedVersion + events.Count;
                
                context.SaveChanges();

                // set the event numbers based on database generated id
                EnumerableExtensions.ForEach(events,stream.Events,(e1,e2) => e1.EventNumber = e2.EventNumber);
                EnumerableExtensions.ForEach(events, stream.Events, (e1,e2) => e1.Hash = e2.Payload.HashString());
                
                LogEvents(streamDetails,events);
                
                foreach (var e in events)
                    _events.OnNext(new Envelope(e,streamDetails));
            }
        }

        private int GetStreamVersion(StreamDetails stream)
        {
            //var version = _subscriptions.GetStreamVersions(stream).Item1;
            //var version = _streamTracker.Get(stream).Version;
            //return version;
            return stream.Version;
        }

        private int GetStreamBranchVersion(StreamDetails stream)
        {
            //var version = _subscriptions.GetStreamVersions(stream).Item2;
            //var version = _streamTracker.Get(stream).BranchVersion;
            //return version;
            return stream.BranchVersion;
        }

        private IEnumerable<IEvent> ReadStreamEventsForwardFromLiveTimeline(StreamDetails stream, long start, int count)
        {
            return ReadStreamEventsForwardFromTimeline(new StreamDetails(stream) {Timeline = Guid.Empty}, start, count);
        }

        private IEnumerable<IEvent> ReadStreamEventsForwardFromTimeline(StreamDetails stream, long start, int count)
        {
            if (GetStreamVersion(stream) <= start)
                return new List<IEvent>();
            
            using (var db = _eventDb.GetContext())
            {
                var streamId = stream.Name.HashString();
                var streamQuery = db.Set<Stream>().Where(x => x.HashId == streamId && x.TimelineId == stream.Timeline);
                var allEvents = streamQuery.SelectMany(x => x.Events).OrderBy(e => e.Version);

                // we need to also read the events which are applied retroactively
                var iStart = (int) start;
                
                var events = allEvents.Skip(iStart).Take(count);
                // TODO: it will read events just from the stream not global, need to fix
                //var events = allEvents.Where(e => e.EventNumber >= start).Take(count);
                
                // set the event numbers based on database generated id
                //ForEach(events, stream.Events, (e1, e2) => e1.EventNumber = e2.EventNumber);
                
                var now = _timeline.Now();

                var ievents = events.ToList()
                    .Where(e => e.TimestampUtc <= now.ToDateTimeUtc())
                    .Select(_eventSerializer.Deserialize)
                    .OrderBy(e => e.Timestamp);
                
                EnumerableExtensions.ForEach(ievents, events, (e1,e2) => e1.EventNumber = e2.EventNumber );
                EnumerableExtensions.ForEach(ievents, events, (e1,e2) => e1.Hash = e2.Payload.HashString());
                return ievents;
            } 
        }
        
        // start is usually aggregate version number
        // if we have inserted a historical event its version will be n+1
        // timestamp would place it somewhere between k and k+1
        // when doing correct historical replay ( start = -1 ) up to timestamp(k+1) 
        // we will have all events filtered by that timestamp
        // so it will include (1..k,n+1) events
        // then they need to be reordered by timestamp to be applied in correct order
        public IEnumerable<IEvent> ReadStreamEventsForward(StreamDetails stream, long start, int count)
        {
            return ReadStreamEventsForwardFromTimeline(stream,start, count);
            //return ReadStreamEventsForwardFromTimeline(new StreamDetails(stream) {Timeline = _timeline.TimelineId},
            //    start, count);
        }
    }
}