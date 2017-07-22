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
    public class StreamAddedArgs : EventArgs
    {
        public string StreamName { get; private set; }

        public StreamAddedArgs(string name)
        {
            StreamName = name;
        }
    }

    public class EventAppendedArgs : EventArgs
    {
        public string StreamName { get; private set; }
        public IEvent Event { get; private set; }

        public EventAppendedArgs(string name, IEvent e)
        {
            StreamName = name;
            Event = e;
        }
    }

    public class SqlStoreConnection : IEventStoreConnection
    {
        private readonly IEventDb _eventDb;
        private readonly bool _inMemory;
        private readonly ISerializer _serializer;
        private readonly ITimeline _timeline;
        private readonly Dictionary<Action<IEvent>, EventAppendedHandler> _subscriptions = new Dictionary<Action<IEvent>,EventAppendedHandler>();

        public delegate void EventAppendedHandler(object sender, EventAppendedArgs e);

        public delegate void StreamAddedHandler(object sender, StreamAddedArgs e);
        private event EventAppendedHandler EventAppended;
        public event StreamAddedHandler StreamAdded;

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

        public bool Exists(string streamName)
        {
            using (var db = _eventDb.GetContext())
            {
                //db.LogToConsole();
                var streamId = streamName.GetHashCode();
                return db.Set<Stream>().AsNoTracking().Any(x => x.HashId == streamId);
            }
        }

        public void Initialise()
        {
            _eventDb.Init();
        }

        public IEnumerable<string> GetStreams<T>()
        {
            using (var context = _eventDb.GetContext())
                return context.Set<Stream>().Where(x => x.SourceType == typeof(T).Name).Select( x => x.Name ).ToList();
        }

        private IEnumerable<Stream> GetStreams(Type sourceType)
        {
            using (var context = _eventDb.GetContext())
                return context.Set<Stream>().Where(x => x.SourceType == sourceType.Name);
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
                    SourceType = details.SourceType?.Name,
                    Version = 0
                };
                context.Set<Stream>().Add(stream);
                StreamAdded?.Invoke(this, new StreamAddedArgs(streamName));
            }
            else
            {
                // create a dummy stream to avoid loading all events from database
                stream = new Stream { HashId = streamName.GetHashCode(), Name = streamName, SourceType = details.SourceType?.Name, Version = version };
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
                var stream = OpenStreamForWriting(context, new StreamDetails { Name = "Null" });
                TimestampEvents(events);
                WriteStream(stream, events);

                context.SaveChanges();
            }
        }

        public void AppendToStream(StreamDetails details,int expectedVersion, IEnumerable<IEvent> enumerable)
        {
            AppendToStream(c => OpenStreamForWriting(c,details), expectedVersion, enumerable);
        }

        public void AppendToStream(string streamName, int expectedVersion, IEnumerable<IEvent> enumerable)
        {
            AppendToStream(c => OpenStreamForWriting(c, new StreamDetails { Name = streamName }), expectedVersion, enumerable);
        }

        private void WriteStream(Stream stream, IList<IEvent> events)
        {
            var eventsDto = events.Select(Serialize).ToList();

            foreach (var e in eventsDto)
            {
                stream.Events.Add(e);
                stream.Version++;
                Debug.WriteLine(stream.Name + " : " + e.Payload);
            }

        }

        private void UpdateSubscribers(Stream stream, IEvent e)
        {
            EventAppended?.Invoke(this, new EventAppendedArgs(stream.Name, e));
        }

        private void AppendToStream(Func<DbContext,Stream> streamSelector, int expectedVersion, IEnumerable<IEvent> enumerable)
        {
            var events = enumerable as IList<IEvent> ?? enumerable.ToList();
            if (!events.Any())
                return;

            using (var context = _eventDb.GetContext())
            {
                var stream = streamSelector(context);

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

                foreach (var e in events)
                    UpdateSubscribers(stream, e);
            }
        }

        private int GetStreamVersion(string name)
        {
            using (var db = _eventDb.GetContext())
            {
                var streamId = name.GetHashCode();
                var streams = db.Set<Stream>().AsNoTracking().Where(x => x.HashId == streamId).Select(x => x.Version).ToList();

                if (!streams.Any())
                    return -1;
                return streams.Single();
            }
        }

        public IEnumerable<IEvent> ReadStreamEventsForward(string streamName, long start, int count)
        {
            if (GetStreamVersion(streamName) == -1)
                return new List<IEvent>();

            using (var context = _eventDb.GetContext())
            {
                //context.LogToConsole();
                var streamId = streamName.GetHashCode();
                var streamQuery = context.Set<Stream>().Where(x => x.HashId == streamId);

                if (_inMemory)
                    streamQuery = streamQuery.Include(x => x.Events);

                var stream = streamQuery.SingleOrDefault();

                var allEvents = _inMemory ? stream.Events : context.Entry(stream).Collection(x => x.Events).Query().AsEnumerable();

                var events = allEvents.Skip((int)start).Take(count)
                        .ToList();

                var now = _timeline.Now();

                return events.Select(Deserialize)
                    .Where(e => e.Timestamp.CompareTo(now) <= 0)
                    .OrderBy(e => e.Timestamp);
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
        
        public void SubscribeToStreams<T>(int eventNumber, Action<IEvent> action)
        {
            foreach(var streamName in GetStreams(typeof(T)).Select(x => x.Name))
                SubscribeToStream(streamName,eventNumber,action);
        }

        public void SubscribeToStream(string streamName, int eventNumber, Action<IEvent> action)
        {
            var now = _timeline.Now();
            var events = ReadStreamEventsForward(streamName, eventNumber, int.MaxValue).Where(e => now.CompareTo(e.Timestamp) >= 0)
                .ToList().OrderBy(e => e.Timestamp);

            foreach (var e in events)
                action(e);

            _subscriptions[action] = (o, e) =>
            {
                if (e.StreamName == streamName && _timeline.Now().CompareTo(e.Event.Timestamp) >= 0)
                    action(e.Event);
            };

            EventAppended += _subscriptions[action];
        }


        public void DropSubscription(string streamName, Action<IEvent> action)
        {
            EventAppended -= _subscriptions[action];
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