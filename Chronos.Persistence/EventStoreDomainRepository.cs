using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Misc;
using NodaTime;

namespace Chronos.Persistence
{

    public static class AggregateExtensions
    {
        public static int ExpectedVersion(this IAggregate aggregate, IEnumerable<IEvent> events)
        {
            return aggregate.Version - events.Count();
        }
    }
    /// <summary>
    /// The event sourcing store
    /// </summary>
    public class EventStoreDomainRepository : IDomainRepository
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStoreConnection _connection;
        private readonly HashSet<Guid> _existingAggregates = new HashSet<Guid>();
        private readonly Dictionary<Type, IAggregate> _lastAggregates = new Dictionary<Type, IAggregate>();

        public EventStoreDomainRepository(IEventBus eventBus, IEventStoreConnection connection)
        {
            _eventBus = eventBus;
            _connection = connection;
        }

        public void Save<T>(T aggregate) where T :IAggregate
        {
            var events = aggregate.UncommitedEvents.ToList();
            if (!events.Any())
                return;
            var expectedVersion = aggregate.ExpectedVersion(events);

            _connection.AppendToStream(aggregate.StreamName(), expectedVersion, events);
            _lastAggregates[typeof(T)] = aggregate;

            aggregate.ClearUncommitedEvents();

            foreach (dynamic e in events)
                _eventBus.Publish(e);
        }

        public T Find<T>(Guid id) where T : IAggregate
        {
            if (_lastAggregates.ContainsKey(typeof(T)) && _lastAggregates[typeof(T)].Id == id)
                return (T) _lastAggregates[typeof(T)];

            var streamName = StreamExtensions.StreamName<T>(id);
            var events = _connection.ReadStreamEventsForward(streamName, 0, int.MaxValue).AsCachedAnyEnumerable();

            if (events.Any())
            {
                var aggregate = (T) Activator.CreateInstance(typeof(T), id, events);
                _lastAggregates[typeof(T)] = aggregate;
                return aggregate;
            }

            return default(T);
        }

        public T Get<T>(Guid id) where T : IAggregate
        {
            var entity = Find<T>(id);
            if (entity == null)
                throw new InvalidOperationException("No events recorded for aggregate");

            return entity;
        }

        public bool Exists<T>(Guid id) where T : IAggregate
        {
            if (_lastAggregates.ContainsKey(typeof(T)) && _lastAggregates[typeof(T)].Id == id || _existingAggregates.Contains(id))
            {
                _existingAggregates.Add(id);
                return true;
            }
            return Find<T>(id) != null;
        }

        public void Replay(Instant date)
        {
            // the events should be resorted by timestamp as we might have modified the past
            var events = _connection.GetAllEvents().Where(e => e.Timestamp.CompareTo(date) <= 0).ToList()
                .OrderBy(e => e.Timestamp);

            _eventBus.Publish(new ReplayStarted());

            foreach(dynamic e in events)
                _eventBus.Publish(e);

            _eventBus.Publish(new ReplayFinished());
        }
    }
}