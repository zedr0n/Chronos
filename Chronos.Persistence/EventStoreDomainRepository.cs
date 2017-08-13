using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using NodaTime;
using Remotion.Linq.Clauses.ResultOperators;
using StreamExtensions = Chronos.Infrastructure.StreamExtensions;

namespace Chronos.Persistence
{
    /// <summary>
    /// The event sourcing store
    /// </summary>
    public class EventStoreDomainRepository : IDomainRepository
    {
        private readonly IEventStoreConnection _connection;
        private readonly IDebugLog _debugLog;

        public EventStoreDomainRepository(IEventStoreConnection connection, IDebugLog debugLog)
        {
            _connection = connection;
            _debugLog = debugLog;
        }

        private readonly Cache _cache = new Cache();

        private class Cache
        {
            private readonly Dictionary<Guid,IAggregate> _dictionary = new Dictionary<Guid, IAggregate>();

            public T Get<T>(Guid id)
                where T : class,IAggregate
            {
                if (_dictionary.ContainsKey(id))
                    return _dictionary[id] as T;
                return null;
            }

            public void Set<T>(T aggregate)
                where T : class, IAggregate
            {
                _dictionary[aggregate.Id] = aggregate;
            }
        }


        public void Save<T>(T aggregate) where T :class,IAggregate,new()
        {
            var events = aggregate.UncommitedEvents.ToList();
            if (!events.Any())
                return;

            _debugLog.WriteLine("@" + typeof(T).Name + " : ");

            var stream = new StreamDetails(aggregate);

            _connection.Writer.AppendToStream(stream, aggregate.Version - events.Count, events);

            aggregate.ClearUncommitedEvents();
            _cache.Set(aggregate);

        }

        public void Save<T>(Guid id, IEnumerable<IEvent> events)
        {
            var stream = new StreamDetails(typeof(T),id);
            _connection.Writer.AppendToStream(stream, 0 , events);
        }

        public T Find<T>(Guid id) where T : class,IAggregate, new()
        {
            var cached = _cache.Get<T>(id);
            var version = cached?.Version ?? 0;

            var streamDetails = new StreamDetails(typeof(T),id);
            var events = _connection.Reader.ReadStreamEventsForward(streamDetails.Name, version, int.MaxValue).ToList();

            if (!events.Any() && version == 0)
                return null;

            var aggregate = cached ?? new T();
            aggregate.LoadFrom<T>(id, events);
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
            return _connection.Exists(new StreamDetails(typeof(T),id));
        }

        public void Replay(Instant date)
        {
            // the events should be resorted by timestamp as we might have modified the past
            var events = _connection.Reader.GetAggregateEvents().Where(e => e.Timestamp.CompareTo(date) <= 0)
                .ToList()
                .OrderBy(e => e.Timestamp)
                .ThenBy(e => e.EventNumber);          

            //foreach (dynamic e in events)
            //    _eventBus.Publish(e);

            _connection.Writer.AppendToNull( new[] { new ReplayCompleted { Timestamp = date} });
            //_eventBus.Publish(new ReplayCompleted { Timestamp = date });
        }
    }
}