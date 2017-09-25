using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using Chronos.Persistence.Serialization;
using Chronos.Persistence.Types;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
using Stream = Chronos.Persistence.Types.Stream;

namespace Chronos.Persistence
{
    public partial class SqlStoreConnection : IEventStoreConnection, IEventStoreReader, IEventStoreWriter
    {
        private readonly IEventDb _eventDb;
        private readonly IEventSerializer _serializer;
        private readonly ITimeline _timeline;

        public IEventStoreSubscriptions Subscriptions => _subscriptions;
        public IEventStoreWriter Writer => this;
        public IEventStoreReader Reader => this;
        private readonly EventStoreSubscriptions _subscriptions;
        private readonly IDebugLog _debugLog;

        public SqlStoreConnection(IEventDb eventDb, IEventSerializer serializer, ITimeline timeline, IDebugLog debugLog)
        {
            _eventDb = eventDb;
            _timeline = timeline;
            _debugLog = debugLog;
            _serializer = serializer;
            _subscriptions = new EventStoreSubscriptions(this);
        }

        private void TimestampEvents(IEnumerable<IEvent> events)
        {
            var timestamp = _timeline.Now(); 
            // we only update the timestamps where they haven't been preset
            foreach (var e in events.Where(e => e.Timestamp == default(Instant)))
                e.Timestamp = timestamp;
        }

        public bool Exists(StreamDetails stream)
        {
            return GetStreamVersion(stream) != -1;
        }

        private IEnumerable<StreamDetails> GetStreams()
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
                });
                if (streams.Any(s => s.Timeline == _timeline.TimelineId))
                    streams = streams.Where(s => s.Timeline == _timeline.TimelineId);
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
                stream.Events.AddRange(events.Select(_serializer.Serialize));
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
                .Select(_serializer.Serialize));
        }

        private static void ForEach<T1, T2>(IEnumerable<T1> to, IEnumerable<T2> from, Action<T1,T2> action)
        {
            using (var e1 = to.GetEnumerator())
            using (var e2 = from.GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                    action(e1.Current, e2.Current);
            }
        }

        private void WriteStream(Stream stream, IEnumerable<IEvent> events)
        {
            var eventsDto = events.Select(_serializer.Serialize).ToList();

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
            if (stream.Name.Contains("Saga"))
                return;

            foreach (var e in events)
            {
                _debugLog.WriteLine(e.GetType().Name + "( " +
                                    InstantPattern.ExtendedIso.Format(e.Timestamp) + " )");
                _debugLog.WriteLine(_serializer.Serialize(e).Payload);

            }
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
                    throw new InvalidOperationException("Stream version is not consistent with events : "
                                                         + "version : " + stream.Version + " < " + "expected :" + expectedVersion);
                if (stream.Version > expectedVersion)
                {
                    // check event numbers to see if we are trying to write a past event again
                    var futureEvents = ReadStreamEventsForward(streamDetails, expectedVersion,events.Count).ToList();
                    var futureEventIds = futureEvents.Select(e => e.EventNumber);

                    if (!events.Select(e => e.EventNumber).SequenceEqual(futureEventIds))
                    {
                        if (_timeline.Live) // concurrency exception
                            throw new InvalidOperationException("Trying to change past for stream " + stream.Name +
                                                                " : " + stream.Version + " > " + expectedVersion);
                        // we are inserting in historical mode
                        // what checks can we do to validate such an insertion?
                        if(!events.All(e => e.Insertable))
                            throw new InvalidOperationException("Trying to insert an event which is not insertable " + stream.Name + 
                                                                " : " + stream.Version + " > " + expectedVersion);                        
                    }
                }

                TimestampEvents(events);
                WriteStream(stream, events);
                
                streamDetails.Version = stream.Version; //expectedVersion + events.Count;
                
                context.SaveChanges();

                // set the event numbers based on database generated id
                ForEach(events,stream.Events,(e1,e2) => e1.EventNumber = e2.EventNumber);
                LogEvents(streamDetails,events);
                
                foreach (var e in events)
                    _subscriptions.OnEventAppended(streamDetails, e);
                
                // if no other events were present in the stream
                // this needs to happen after  
                // as otherwise events will be double processed
                // here and in OnEventAppended subscribers
                if (streamDetails.Version == events.Count)
                    _subscriptions.OnStreamAdded(streamDetails);
            }
        }

        private int GetStreamVersion(StreamDetails stream)
        {
            var version = _subscriptions.GetStreamVersions(stream).Item1;
            return version;
        }

        private int GetStreamBranchVersion(StreamDetails stream)
        {
            var version = _subscriptions.GetStreamVersions(stream).Item2;
            return version;
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
                    .Select(_serializer.Deserialize)
                    .OrderBy(e => e.Timestamp);
                
                ForEach(ievents, events, (e1,e2) => e1.EventNumber = e2.EventNumber );
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
            return ReadStreamEventsForwardFromTimeline(new StreamDetails(stream) {Timeline = _timeline.TimelineId},
                start, count);
        }
    }
}