using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Misc;
using NodaTime;

namespace Chronos.Persistence
{
    /// <summary>
    /// The event sourcing store
    /// </summary>
    public class EventStoreDomainRepository : DomainRepositoryBase
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStoreConnection _connection;

        public EventStoreDomainRepository(IEventBus eventBus, IEventStoreConnection connection)
        {
            _eventBus = eventBus;
            _connection = connection;
        }

        public override void Save<T>(T aggregate)
        {
            var events = aggregate.UncommitedEvents.ToList();
            var expectedVersion = aggregate.ExpectedVersion(events);

            _connection.AppendToStream(aggregate.StreamName(), expectedVersion, events);

            aggregate.ClearUncommitedEvents();

            foreach (dynamic e in events)
                _eventBus.Publish(e);
        }

        public override T Find<T>(Guid id) 
        {
            var streamName = StreamExtensions.StreamName<T>(id);
            var events = _connection.ReadStreamEventsForward(streamName, 0, int.MaxValue).AsCachedAnyEnumerable();

            if (events.Any())
                return (T)Activator.CreateInstance(typeof(T), id, events);

            return default(T);
        }

        public override void Replay(Instant date)
        {
            var events = _connection.GetAllEvents().Where(e => e.Timestamp.CompareTo(date) <= 0).ToList();
            foreach(dynamic e in events)
                _eventBus.Publish(e);
        }
    }
}