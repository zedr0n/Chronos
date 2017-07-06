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
        private readonly IEventBus _eventBus;
        //private readonly IEventDb _eventDb;
        private readonly IEventStoreConnection _connection;


        public EventStoreDomainRepository(IEventBus eventBus, IEventStoreConnection connection)
        {
            _eventBus = eventBus;
            _connection = connection;
        }

        public override void Save<T>(T aggregate) 
        {
            var events = aggregate.UncommitedEvents.ToList();

            var expectedVersion = CalculateExpectedVersion(aggregate, events);
            var streamName = StreamExtensions.AggregateToStreamName(typeof(T), aggregate.Id);

            _connection.AppendToStream(streamName,expectedVersion,events);

            aggregate.ClearUncommitedEvents();

            foreach (dynamic e in events)
                _eventBus.Publish(e);
        }
        public override T Find<T>(Guid id) 
        {
            var streamName = StreamExtensions.AggregateToStreamName(typeof(T), id);
            var events = _connection.ReadStreamEventsForward(streamName, 0, int.MaxValue).AsCachedAnyEnumerable();

            if (events.Any())
                return (T)Activator.CreateInstance(typeof(T), id, events);

            return default(T);
        }
    }
}