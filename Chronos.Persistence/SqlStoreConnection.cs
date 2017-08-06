using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using Chronos.Persistence.Serialization;
using Chronos.Persistence.Types;
using Microsoft.EntityFrameworkCore;
using NodaTime.Text;
using Stream = Chronos.Persistence.Types.Stream;

namespace Chronos.Persistence
{
    public partial class SqlStoreConnection : IEventStoreConnection, IEventStoreReader, IEventStoreWriter
    {
        private readonly IEventDb _eventDb;
        private readonly bool _inMemory;
        private readonly IEventSerializer _serializer;
        private readonly ITimeline _timeline;

        public IEventStoreSubscriptions Subscriptions => _subscriptions;
        public IEventStoreWriter Writer => this;
        public IEventStoreReader Reader => this;
        private readonly EventStoreSubscriptions _subscriptions;
        private readonly IDebugLog _debugLog;

        public SqlStoreConnection(IEventDb eventDb, IEventSerializer serializer, bool inMemory, ITimeline timeline, IDebugLog debugLog)
        {
            _eventDb = eventDb;
            _inMemory = inMemory;
            _timeline = timeline;
            _debugLog = debugLog;
            _serializer = serializer;
            _subscriptions = new EventStoreSubscriptions(this);
        }

        private void TimestampEvents(IEnumerable<IEvent> events)
        {
            var timestamp = _timeline.Now();
            foreach (var e in events)
                e.Timestamp = timestamp;
        }

        public bool Exists(StreamDetails stream)
        {
            using (var db = _eventDb.GetContext())
            {
                //db.LogToConsole();
                var streamId = stream.Name.GetHashCode();
                return db.Set<Stream>().AsNoTracking().Any(x => x.HashId == streamId);
            }
        }

        public IEnumerable<StreamDetails> GetStreams()
        {
            using (var context = _eventDb.GetContext())
            {
                var streams = context.Set<Stream>().Select(s => new StreamDetails(s.SourceType,s.Key));
                return streams.ToList();
            };
        }

        private Stream OpenStreamForWriting(DbContext context, StreamDetails details)
        {
            var streamName = details.Name;
            var version = GetStreamVersion(streamName);

            Stream stream;

            if (version == -1)
            {
                // create a new stream if none exists
                stream = new Stream
                {
                    HashId = streamName.GetHashCode(),
                    Name = streamName,
                    SourceType = details.SourceType,
                    Key = details.Key,
                    Version = 0
                };
                context.Set<Stream>().Add(stream);
            }
            else
            {
                // create a dummy stream to avoid loading all events from database
                stream = new Stream
                {
                    HashId = streamName.GetHashCode(),
                    Name = streamName,
                    SourceType = details.SourceType,
                    Key = details.Key,
                    Version = version
                };
                context.Set<Stream>().Attach(stream);
            }

            return stream;
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

        public void AppendToNull(IEnumerable<IEvent> enumerable)
        {
            var events = enumerable as IList<IEvent> ?? enumerable.ToList();
            if (!events.Any())
                return;

            using (var context = _eventDb.GetContext())
            {
                var details = new StreamDetails("Global");
                var stream = OpenStreamForWriting(context, details);
                TimestampEvents(events);
                WriteStream(stream, events);

                context.SaveChanges();

                // set the event numbers based on database generated id
                ForEach(events, stream.Events, (e1, e2) => e1.EventNumber = e2.EventNumber);
                LogEvents(details,events);
            }
        }

        private void WriteStream(Stream stream, IEnumerable<IEvent> events)
        {
            var eventsDto = events.Select(_serializer.Serialize).ToList();

            foreach (var e in eventsDto)
            {
                stream.Events.Add(e);
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

            using (var context = _eventDb.GetContext())
            {
                var stream = OpenStreamForWriting(context, streamDetails);
                var streamAdded = stream.Version == 0;

                if (stream.Version < expectedVersion)
                    throw new InvalidOperationException("Stream version is not consistent with events : "
                                                         + "version : " + stream.Version + " < " + "expected :" + expectedVersion);
                if (stream.Version > expectedVersion)
                {
                    // check event numbers to see if we are trying to write a past event again
                    var futureEvents = ReadStreamEventsForward(stream.Name, expectedVersion,events.Count()).ToList();
                    var futureEventIds = futureEvents.Select(e => e.EventNumber);
                    if(!events.Select(e => e.EventNumber).SequenceEqual(futureEventIds))
                        throw new InvalidOperationException("Trying to change past for stream " + stream.Name + " : "
                                                            + stream.Version + " > " + expectedVersion);
                }

                TimestampEvents(events);
                WriteStream(stream,events);

                context.SaveChanges();

                // set the event numbers based on database generated id
                ForEach(events,stream.Events,(e1,e2) => e1.EventNumber = e2.EventNumber);
                LogEvents(streamDetails,events);

                // if no other events were present in the stream
                if (streamAdded && !streamDetails.SourceType.Contains("Saga"))
                    _subscriptions.OnStreamAdded(streamDetails);
                foreach (var e in events)
                    _subscriptions.OnEventAppended(streamDetails, e);
            }
        }

        private int GetStreamVersion(string name)
        {
            using (var db = _eventDb.GetContext())
            {
                //db.LogToConsole();
                var streamId = name.GetHashCode();
                var streamVersion = db.Set<Stream>().AsNoTracking()
                    .Where(x => x.HashId == streamId)
                    .Select(x => x.Version)
                    .ToList();
                //db.StopLogging();

                if (!streamVersion.Any())
                    return -1;

                return streamVersion.Single();
            }
        }

        private bool StreamExists(string name)
        {
            using (var db = _eventDb.GetContext())
            {
                var streamId = name.GetHashCode();
                //return db.Set<Stream>().Find(streamId) != null;
                return db.Set<Stream>().Any(x => x.HashId == streamId);
            }
        }

        public IEnumerable<IEvent> ReadStreamEventsForward(string streamName, long start, int count)
        {
            //if (GetStreamVersion(streamName) == -1)
            //    return new List<IEvent>();

            //if (!StreamExists(streamName))
            //    return new List<IEvent>();
            
            using (var db = _eventDb.GetContext())
            {
                //if (!StreamExists(db, streamName))
                //    return new List<IEvent>();
                
                var streamId = streamName.GetHashCode();
                var streamQuery = db.Set<Stream>().Where(x => x.HashId == streamId);

                //if (!streamQuery.Any())
                //    return new List<IEvent>();

                if (_inMemory)
                    streamQuery = streamQuery.Include(x => x.Events);

                var allEvents = streamQuery.SelectMany(x => x.Events).OrderBy(e => e.EventNumber);

                //var allEvents = _inMemory
                //    ? stream
                //    : context.Entry(stream).Collection(x => x.Events).Query();//.OrderBy(e => e.EventNumber);

                //context.Set<Stream>().Skip(() => start);
                var iStart = (int) start;
                var events = allEvents.Skip(iStart).Take(count);

                var now = _timeline.Now();

                return events.ToList()
                    .Where(e => e.TimestampUtc <= now.ToDateTimeUtc())
                    .Select(_serializer.Deserialize)
                    .OrderBy(e => e.Timestamp);
                    
                
                return events.Select(_serializer.Deserialize)
                    .Where(e => e.Timestamp <= now)
                    .OrderBy(e => e.Timestamp)
                    .ToList();
            }
        }

        public IEnumerable<IEvent> GetAggregateEvents()
        {
            using (var context = _eventDb.GetContext())
            {
                var events = new List<IEvent>();
                foreach (var stream in context.Set<Stream>().Where(s => !s.Name.Contains("Saga")).Include(x => x.Events).AsEnumerable())
                    events.AddRange(stream.Events.Select(_serializer.Deserialize));

                return events;
            }
        }
        
    }
}