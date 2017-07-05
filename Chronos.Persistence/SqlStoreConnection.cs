using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public class SqlStoreConnection : IEventStoreConnection
    {

        private readonly IEventDb _eventDb;

        public SqlStoreConnection(IEventDb eventDb)
        {
            _eventDb = eventDb;
        }

        public void AppendToStream(string streamName, int expectedVersion, IEnumerable<Event> events)
        {
            _eventDb.Init();
            using (var context = _eventDb.GetContext())
            {
                var stream = context.Set<Stream>().SingleOrDefault(x => x.Name == streamName) ?? new Stream
                {
                    Name = streamName,
                    Version = 0,
                    Events = new List<Event>()
                };

                if (stream.Version != expectedVersion)
                    throw new InvalidOperationException("Stream version is not consistent with events");

                var enumerable = events as IList<Event> ?? events.ToList();

                foreach (var e in enumerable)
                {
                    stream.Events.Add(e);
                    Debug.Write(stream.Name + " : " + e.Payload);
                }

                context.SaveChanges();
            }
        }

        public IEnumerable<Event> ReadStreamEventsForward(string streamName, long start, int count)
        {
            _eventDb.Init();
            using (var context = _eventDb.GetContext())
            {
                var stream = context.Set<Stream>().SingleOrDefault(x => x.Name == streamName);
                var events = context.Entry(stream).Collection(x => x.Events).Query().Skip((int) start - 1).Take(count);

                return events;
            }
        }
    }
}