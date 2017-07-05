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
    /// <typeparam name="T">The aggregate type</typeparam>
    public class DomainRepository : IDomainRepository
    {
        private readonly ISerializer _serializer;
        private readonly IEventBus _eventBus;
        private readonly IEventDb _eventDb;


        public DomainRepository(ISerializer serializer, IEventBus eventBus, IEventDb eventDb)
        {
            _serializer = serializer;
            _eventBus = eventBus;
            _eventDb = eventDb;
        }

        /// <summary>
        /// Saves the aggregate events to event store and publish those to event bus
        /// </summary>
        /// <param name="aggregate">The aggregate instance</param>
        public void Save<T>(T aggregate) where T : IAggregate
        {
            _eventDb.Init();
            var events = aggregate.UncommitedEvents.ToList();
            using (var context = _eventDb.GetContext())
            {
                var eventsSet = context.Set<Event>();
                foreach (var e in events)
                {
                    var @event = Serialize<T>(e, "");
                    eventsSet.Add(@event);
                    Debug.Write(@event.Type + " : " + @event.Payload);
                }

                context.SaveChanges();
            }
            aggregate.ClearUncommitedEvents();

            foreach (dynamic e in events)
                _eventBus.Publish(e);
        }
        /// <summary>
        /// Rebuild the aggregate from event history extracted from Event Store
        /// </summary>
        /// <param name="id">The aggregate guid</param>
        /// <returns>Aggregate or null if no events found</returns>
        public T Find<T>(Guid id) where T : IAggregate
        {
            _eventDb.Init();
            using (var context = _eventDb.GetContext())
            {
                var deserialized = context.Set<Event>().Where(x => x.Guid == id && x.Type == typeof(T).ToString())
                    .OrderBy(x => x.Version)
                    .AsEnumerable()
                    .Select(Deserialize)
                    .AsCachedAnyEnumerable();

                if (deserialized.Any())
                    return (T) Activator.CreateInstance(typeof(T), id, deserialized);

                return default(T);
            }
        }
        /// <summary>
        /// Rebuilds the aggregate from event history extracted from Event Store
        /// </summary>
        /// <exception cref="ArgumentException">if no events found for aggregate with this id</exception>
        /// <param name="id">The aggregate guid</param>
        /// <returns></returns>
        public T Get<T>(Guid id) where T : IAggregate
        {
            var entity = Find<T>(id);
            if (entity == null)
                throw new ArgumentException();

            return entity;
        }

        private Event Serialize<T>(IEvent e, string correlationId)
        {
            Event serialized;

            using (var writer = new StringWriter())
            {
                _serializer.Serialize(writer, e);

                serialized = new Event
                {
                    Type = typeof(T).ToString(),
                    Guid = e.SourceId,
                    Timestamp = e.Timestamp.ToDateTimeUtc(),
                    Payload = writer.ToString(),
                    CorrelationId = correlationId
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