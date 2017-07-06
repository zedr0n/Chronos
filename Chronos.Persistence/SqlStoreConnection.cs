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
        private readonly bool _inMemory;

        public SqlStoreConnection(IEventDb eventDb, bool inMemory)
        {
            _eventDb = eventDb;
            _inMemory = inMemory;
        }

        public void AppendToStream(string streamName, int expectedVersion, IEnumerable<Event> events)
        {
            _eventDb.Init();
            using (var context = _eventDb.GetContext())
            {
                var streamQuery = context.Set<Stream>().AsNoTracking().Where(x => x.Name == streamName);
                var stream = streamQuery.SingleOrDefault();

                var enumerable = events as IList<Event> ?? events.ToList();

                if (stream == null)
                {
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
                    stream = new Stream {Name = streamName, Version = stream.Version};
                    context.Set<Stream>().Attach(stream);
                }

                if (stream.Version != expectedVersion)
                    throw new InvalidOperationException("Stream version is not consistent with events");

                //var streamEvents = context.Entry(stream).Collection(x => x.Events).Query().Take(0).ToList();
                foreach (var e in enumerable)
                {
                    stream.Events.Add(e);
                    stream.Version++;
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
                var streamQuery = context.Set<Stream>().Where(x => x.Name == streamName);
                if (_inMemory)
                    streamQuery = streamQuery.Include(x => x.Events);
                var stream = streamQuery.SingleOrDefault();
                if (stream == null)
                    return new List<Event>();
                    
                var events = context.Entry(stream).Collection(x => x.Events).Query().Skip((int) start - 1).Take(count);

                return events.ToList();
            }
        }
    }
}