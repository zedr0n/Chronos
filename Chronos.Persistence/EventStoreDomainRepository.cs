using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Aggregates;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Misc;
using Chronos.Persistence.Serialization;

namespace Chronos.Persistence
{
    /// <summary>
    /// The event sourcing store
    /// </summary>
    public class EventStoreDomainRepository : DomainRepositoryBase
    {
        private readonly ISerializer _serializer;
        private readonly IEventBus _eventBus;
        //private readonly IEventDb _eventDb;
        private readonly IEventStoreConnection _connection;


        public EventStoreDomainRepository(ISerializer serializer, IEventBus eventBus, IEventStoreConnection connection)
        {
            _serializer = serializer;
            _eventBus = eventBus;
            _connection = connection;
        }

        public override void Save<T>(T aggregate) 
        {
            var events = aggregate.UncommitedEvents.ToList();

            var expectedVersion = CalculateExpectedVersion(aggregate, events);
            var eventsDto = events.Select(Serialize<T>);
            var streamName = AggregateToStreamName(typeof(T), aggregate.Id);

            _connection.AppendToStream(streamName,expectedVersion,eventsDto);

            aggregate.ClearUncommitedEvents();

            foreach (dynamic e in events)
                _eventBus.Publish(e);
        }
        public override T Find<T>(Guid id) 
        {
            var streamName = AggregateToStreamName(typeof(T), id);
            var events = _connection.ReadStreamEventsForward(streamName, 0, int.MaxValue).Select(Deserialize).AsCachedAnyEnumerable();

            if (events.Any())
                return (T)Activator.CreateInstance(typeof(T), id, events);

            return default(T);
        }

        private static string AggregateToStreamName(Type type, Guid id)
        {
            return string.Format("{0}-{1}", type.Name, id);
        }

        private Event Serialize<T>(IEvent e)
        {
            Event serialized;

            using (var writer = new StringWriter())
            {
                _serializer.Serialize(writer, e);

                serialized = new Event
                {
                    Guid = e.SourceId,
                    Timestamp = e.Timestamp.ToDateTimeUtc(),
                    Payload = writer.ToString()
                };
            }

            return serialized;
        }
        private IEvent Deserialize(Event e)
        {
            using (var reader = new StringReader(e.Payload))
                return _serializer.Deserialize<IEvent>(reader);
        }
    }
}