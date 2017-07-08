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


        public SqlStoreConnection(IEventDb eventDb, ISerializer serializer, bool inMemory)
        {
            _eventDb = eventDb;
            _inMemory = inMemory;
            _serializer = serializer;
        }

        private static Stream OpenStreamForWriting(DbContext context, string streamName)
        {
            var streamQuery = context.Set<Stream>().AsNoTracking().Where(x => x.Name == streamName);
            var stream = streamQuery.SingleOrDefault();

            if (stream == null)
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
                stream = new Stream { Name = streamName, Version = stream.Version };
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
            _eventDb.Init();
            using (var context = _eventDb.GetContext())
            {
                var enumerable = events as IList<IEvent> ?? events.ToList();
                var stream = OpenStreamForWriting(context, streamName);

                if (stream.Version != expectedVersion)
                    throw new InvalidOperationException("Stream version is not consistent with events");

                var eventsDto = enumerable.Select(Serialize).ToList();

                foreach (var e in eventsDto)
                {
                    stream.Events.Add(e);
                    stream.Version++;
                    Debug.Write(stream.Name + " : " + e.Payload);
                }

                context.SaveChanges();

                // set the event numbers based on database generated id
                ForEach(enumerable,stream.Events,(e1,e2) => e1.EventNumber = e2.EventNumber);
            }
        }

        public IEnumerable<IEvent> ReadStreamEventsForward(string streamName, long start, int count)
        {
            _eventDb.Init();
            using (var context = _eventDb.GetContext())
            {
                var streamQuery = context.Set<Stream>().Where(x => x.Name == streamName);
                if (_inMemory)
                    streamQuery = streamQuery.Include(x => x.Events);
                var stream = streamQuery.SingleOrDefault();
                if (stream == null)
                    return new List<IEvent>();
                    
                var events = context.Entry(stream).Collection(x => x.Events).Query().Skip((int) start).Take(count);

                return events.ToList().Select(Deserialize);
            }
        }

        public IEnumerable<IEvent> GetAllEvents()
        {
            _eventDb.Init();
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