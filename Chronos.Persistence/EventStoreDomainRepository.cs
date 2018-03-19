using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using NodaTime;
using Remotion.Linq.Clauses.ResultOperators;

namespace Chronos.Persistence
{
    /// <summary>
    /// Event store backed domain repository
    /// </summary>
    public class EventStoreDomainRepository : IDomainRepository
    {
        //private readonly IEventStoreConnection _connection;
        private readonly IEventStore _eventStore;
        private readonly IDebugLog _debugLog;
        private readonly IAggregateFactory _aggregateFactory;
        private readonly StreamTracker _streamTracker;

        public EventStoreDomainRepository(IDebugLog debugLog, IAggregateFactory aggregateFactory, IEventStore eventStore, StreamTracker streamTracker)
        {
            //_connection = connection;
            _debugLog = debugLog;
            _aggregateFactory = aggregateFactory;
            _eventStore = eventStore;
            _streamTracker = streamTracker;
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

            public void Reset()
            {
                _dictionary.Clear();
            }
        }

        public void Save<T>(T aggregate) where T :class,IAggregate
        {
            if (aggregate == null)
                return;
            
            var events = aggregate.UncommitedEvents.ToList();
            if (!events.Any())
                return;

            var expectedVersion = aggregate.Version - events.Count;            
            
            _debugLog.WriteLine("@" + typeof(T).SerializableName() + " : ");

            StreamDetails stream;
            if (expectedVersion == 0)
                stream = _streamTracker.Add(aggregate);
            else 
                stream = _streamTracker.Get(aggregate);

            _eventStore.Connection.AppendToStream(stream,expectedVersion,events);

            aggregate.ClearUncommitedEvents();
            _cache.Set(aggregate);
        }

        public void Save<T>(Guid id, IEnumerable<IEvent> events)
        {
            var stream = _streamTracker.Add(typeof(T), id);
            _eventStore.Connection.AppendToStream(stream, 0 , events);
        }

        public T Find<T>(Guid id) where T : class,IAggregate,new()
        {
            var cached = _cache.Get<T>(id);
            var version = cached?.Version ?? 0;

            var stream = _streamTracker.Get<T>(id);
            if (stream == null)
                return null;
            
            var events = _eventStore.Connection.ReadStreamEventsForward(stream , version, int.MaxValue).ToList();

            if (!events.Any() && version == 0)
                return null;

            //var aggregate = cached ?? _aggregateFactory.Create<T>(stream.SourceType);
            var aggregate = cached ?? new T();
            aggregate.LoadFrom<T>(id, events);
            return aggregate;
        }

        public T Get<T>(Guid id) where T : class,IAggregate,new()
        {
            var entity = Find<T>(id);
            if (entity == null)
                throw new InvalidOperationException("Aggregate not found, has it been created?");

            return entity;
        }

        public void Reset()
        {
            _cache.Reset();
        }
    }
}