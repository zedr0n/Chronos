using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
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
                    Version = -1
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

        public void AppendToStream(string streamName, int expectedVersion, IEnumerable<IEvent> events)
        {
            using (var context = _eventDb.GetContext())
            {
                var enumerable = events as IList<IEvent> ?? events.ToList();
                //context.LogToConsole();
                var stream = OpenStreamForWriting(context, streamName);
                //context.StopLogging();

                if (stream.Version != expectedVersion)
                    throw new InvalidOperationException("Stream version is not consistent with events");

                TimestampEvents(enumerable);
                var eventsDto = enumerable.Select(Serialize).ToList();

                foreach (var e in eventsDto)
                {
                    stream.Events.Add(e);
                    stream.Version++;
                    Debug.Write(stream.Name + " : " + e.Payload);
                }

                _streamVersions[streamName] = stream.Version;

                context.SaveChanges();

                // set the event numbers based on database generated id
                ForEach(enumerable,stream.Events,(e1,e2) => e1.EventNumber = e2.EventNumber);
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
                IList<Event> events = new List<Event>();

                if (_inMemory || start == 0 && count == int.MaxValue)
                    streamQuery = streamQuery.Include(x => x.Events);

                var stream = streamQuery.SingleOrDefault();
                if (stream != null)
                {
                    if (stream.Events == null)
                        events = context.Entry(stream).Collection(x => x.Events).Query().Skip((int) start).Take(count)
                            .ToList();
                    else
                        events = stream.Events;
                }

                return events.Select(Deserialize);
            }
        }

        public IEnumerable<IEvent> GetAllEvents()
        {
            using (var context = _eventDb.GetContext())
            {
                var events = new List<IEvent>();
                foreach (var stream in context.Set<Stream>().Include(x => x.Events).AsEnumerable())
                    events.AddRange(stream.Events.Select(Deserialize));

                //var events = context.Entry(stream).Collection(x => x.Events).Query().Skip((int)start - 1).Take(count);

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