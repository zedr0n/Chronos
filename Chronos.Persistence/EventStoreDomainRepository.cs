using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Misc;
using NodaTime;
using NodaTime.Text;

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
        private readonly IDebugLog _debugLog;

        public EventStoreDomainRepository(IEventBus eventBus, IEventStoreConnection connection, IDebugLog debugLog)
        {
            _eventBus = eventBus;
            _connection = connection;
            _debugLog = debugLog;
        }

        public void Save<T>(T aggregate) where T :class,IAggregate,new()
        {
            var events = aggregate.UncommitedEvents.ToList();
            if (!events.Any())
                return;

            _connection.AppendToStream(aggregate.StreamDetails(), aggregate.Version - events.Count, events);

            aggregate.ClearUncommitedEvents();

            _debugLog.WriteLine("@" + typeof(T).Name + " : ");
            foreach (var e in events)
                _eventBus.Publish(e);
        }

        public T Find<T>(Guid id) where T : class,IAggregate, new()
        {
            var streamName = StreamExtensions.StreamName<T>(id);
            var events = _connection.ReadStreamEventsForward(streamName, 0, int.MaxValue).ToList();

            if (!events.Any())
                return null;

            var aggregate = new T().LoadFrom<T>(id,events);
            return aggregate;
        }

        public T Get<T>(Guid id) where T : class,IAggregate,new()
        {
            var entity = Find<T>(id);
            if (entity == null)
                throw new InvalidOperationException("No events recorded for aggregate");

            return entity;
        }

        public bool Exists<T>(Guid id) where T : class,IAggregate,new()
        {
            return _connection.Exists(StreamExtensions.StreamName<T>(id));
        }

        public void Replay(Instant date)
        {
            // the events should be resorted by timestamp as we might have modified the past
            var events = _connection.GetAggregateEvents().Where(e => e.Timestamp.CompareTo(date) <= 0)
                .ToList()
                .OrderBy(e => e.Timestamp)
                .ThenBy(e => e.EventNumber);          

            foreach (dynamic e in events)
            {
                e.Replaying = true;
                _eventBus.Publish(e);
                e.Replaying = false;
            }
            _eventBus.Publish(new ReplayCompleted { Timestamp = date });
        }
    }
}