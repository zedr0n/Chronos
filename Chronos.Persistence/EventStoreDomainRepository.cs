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
            if (!events.Any())
                return;
            var expectedVersion = aggregate.ExpectedVersion(events);

            _connection.AppendToStream(aggregate.StreamName(), expectedVersion, events);
            LastAggregates[typeof(T)] = aggregate;

            aggregate.ClearUncommitedEvents();

            foreach (dynamic e in events)
                _eventBus.Publish(e);
        }

        public override T Find<T>(Guid id)
        {
            if (LastAggregates.ContainsKey(typeof(T)) && LastAggregates[typeof(T)].Id == id)
                return (T) LastAggregates[typeof(T)];

            var streamName = StreamExtensions.StreamName<T>(id);
            var events = _connection.ReadStreamEventsForward(streamName, 0, int.MaxValue).AsCachedAnyEnumerable();

            if (events.Any())
            {
                var aggregate = (T) Activator.CreateInstance(typeof(T), id, events);
                LastAggregates[typeof(T)] = aggregate;
                return aggregate;
            }

            return default(T);
        }

        public override void Replay(Instant date)
        {
            var events = _connection.GetAllEvents().Where(e => e.Timestamp.CompareTo(date) <= 0).ToList()
                .OrderBy(e => e.Timestamp);

            _eventBus.Publish(new ReplayStarted());

            foreach(dynamic e in events)
                _eventBus.Publish(e);

            _eventBus.Publish(new ReplayFinished());
        }
    }
}