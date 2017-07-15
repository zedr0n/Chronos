using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Persistence.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public class SqlStoreConnection : IEventStoreConnection
    {
        private readonly IEventDb _eventDb;
        private readonly bool _inMemory;
        private readonly ISerializer _serializer;
        private readonly ITimeline _timeline;
        private readonly Dictionary<string,int> _streamVersions = new Dictionary<string, int>();

        public SqlStoreConnection(IEventDb eventDb, ISerializer serializer, bool inMemory, ITimeline timeline)
        {
            _eventDb = eventDb;
            _inMemory = inMemory;
            _timeline = timeline;
            _serializer = serializer;
        }

        private void TimestampEvents(IEnumerable<IEvent> events)
        {
            var timestamp = _timeline.Now();
            foreach (var e in events)
                e.Timestamp = timestamp;
        }

        public void Initialise()
        {
            _eventDb.Init();
            //return;
            using (var db = _eventDb.GetContext())
            {
                var streams = db.Set<Stream>().AsNoTracking().ToList();
                foreach (var stream in streams)
                    _streamVersions[stream.Name] = stream.Version;
            }
        }

        private Stream OpenStreamForWriting(DbContext context, string streamName)
        {
            var version = GetStreamVersion(streamName);

            Stream stream;

            if (version == -1)
            {
                // create a new stream if none exists
                stream = new Stream
                {
                    Name = streamName,
                    Version = 0
                };
                context.Set<Stream>().Add(stream);
            }
            else
            {
                // create a dummy stream to avoid loading all events from database
                stream = new Stream { Name = streamName, Version = version };
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

            const string streamName = "Null";

            using (var context = _eventDb.GetContext())
            {
                var stream = OpenStreamForWriting(context, streamName);
                TimestampEvents(events);
                WriteStream(stream, events);

                context.SaveChanges();
            }
        }

        private void WriteStream(Stream stream, IEnumerable<IEvent> events)
        {
            var eventsDto = events.Select(Serialize).ToList();

            foreach (var e in eventsDto)
            {
                stream.Events.Add(e);
                stream.Version++;
                Debug.WriteLine(stream.Name + " : " + e.Payload);
            }
            _streamVersions[stream.Name] = stream.Version;
        }

        public void AppendToStream(string streamName, int expectedVersion, IEnumerable<IEvent> enumerable)
        {
            var events = enumerable as IList<IEvent> ?? enumerable.ToList();
            if (!events.Any())
                return;

            using (var context = _eventDb.GetContext())
            {
                var stream = OpenStreamForWriting(context, streamName);

                if (stream.Version < expectedVersion)
                    throw new InvalidOperationException("Stream version is not consistent with events : "
                                                         + "version : " + stream.Version + " < " + "expected :" + expectedVersion);
                if (stream.Version > expectedVersion)
                {
                    // check event numbers to see if we are trying to write a past event again
                    var futureEvents = ReadStreamEventsForward(streamName, expectedVersion,events.Count()).ToList();
                    var futureEventIds = futureEvents.Select(e => e.EventNumber);
                    if(!events.Select(e => e.EventNumber).SequenceEqual(futureEventIds))
                        throw new InvalidOperationException("Trying to change past for stream " + streamName + " : "
                                                            + stream.Version + " > " + expectedVersion);
                }

                TimestampEvents(events);
                WriteStream(stream,events);

                context.SaveChanges();

                // set the event numbers based on database generated id
                ForEach(events,stream.Events,(e1,e2) => e1.EventNumber = e2.EventNumber);
            }
        }

        private int GetStreamVersion(string name)
        {
            if (!_streamVersions.ContainsKey(name))
            {
                return -1;
                using (var db = _eventDb.GetContext())
                {
                    var streams = db.Set<Stream>().AsNoTracking().ToList();
                    var stream = streams.SingleOrDefault(s => s.Name == name);
                    if (stream == null)
                        return -1;
                    _streamVersions[name] = stream.Version;
                }
            }
            return _streamVersions[name];
        }

        public IEnumerable<IEvent> ReadStreamEventsForward(string streamName, long start, int count)
        {
            if (GetStreamVersion(streamName) == -1)
                return new List<IEvent>();

            using (var context = _eventDb.GetContext())
            {
                var streamQuery = context.Set<Stream>().Where(x => x.Name == streamName);

                if (_inMemory)
                    streamQuery = streamQuery.Include(x => x.Events);

                var stream = streamQuery.SingleOrDefault();
                var events = context.Entry(stream).Collection(x => x.Events).Query().Skip((int)start).Take(count)
                        .ToList();

                var now = _timeline.Now();

                return events.Select(Deserialize)
                    .Where(e => e.Timestamp.CompareTo(now) <= 0)
                    .OrderBy(e => e.Timestamp);
            }
        }

        public IEnumerable<IEvent> GetAllEvents()
        {
            using (var context = _eventDb.GetContext())
            {               
                var events = new List<IEvent>();
                foreach (var stream in context.Set<Stream>()/*.Where(s => !s.Name.Contains("Saga"))*/.Include(x => x.Events).AsEnumerable())
                    events.AddRange(stream.Events.Select(Deserialize));

                return events;
            }

        }

        public IEnumerable<IEvent> GetAggregateEvents()
        {
            using (var context = _eventDb.GetContext())
            {
                var events = new List<IEvent>();
                foreach (var stream in context.Set<Stream>().Where(s => !s.Name.Contains("Saga")).Include(x => x.Events).AsEnumerable())
                    events.AddRange(stream.Events.Select(Deserialize));

                return events;
            }
        }

        private Event Serialize(IEvent e)
        {
            Event serialized;

            using (var writer = new StringWriter())
            {
                _serializer.Serialize(writer, e);

                serialized = new Event
                {
                    Guid = e.SourceId,
                    TimestampUtc = e.Timestamp.ToDateTimeUtc(),
                    Payload = writer.ToString()
                };
            }

            return serialized;
        }
        private IEvent Deserialize(Event e)
        {
            using (var reader = new StringReader(e.Payload))
            {
                var @event = _serializer.Deserialize<IEvent>(reader);
                @event.EventNumber = e.EventNumber;
                return @event;
            }
        }
    }
}