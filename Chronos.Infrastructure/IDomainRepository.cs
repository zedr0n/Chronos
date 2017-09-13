using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure
{
    public interface IDomainRepository
    {
        /// <summary>
        /// Saves the aggregate events to event store and publish those to event bus
        /// </summary>
        /// <param name="aggregate">The aggregate instance</param>
        void Save<T>(T aggregate) where T : class, IAggregate;

        /// <summary>
        ///  Saves the aggregate events directly to event store
        /// Used for BDD testing
        /// </summary>
        /// <param name="id">Aggregate id</param>
        /// <param name="events">Event sequuence to save for aggregate</param>
        /// <typeparam name="T">Aggregate type</typeparam>
        void Save<T>(Guid id, IEnumerable<IEvent> events);

        bool Exists<T>(Guid id) where T : class, IAggregate;

        /// <summary>
        /// Rebuild the aggregate from event history extracted from Event Store
        /// </summary>
        /// <param name="id">The aggregate guid</param>
        /// <typeparam name="T">Aggregate type</typeparam>
        /// <returns>Aggregate or null if no events found</returns>
        T Find<T>(Guid id) where T : class, IAggregate;
        /// <summary>
        /// Rebuilds the aggregate from event history extracted from Event Store
        /// </summary>
        /// <exception cref="ArgumentException">if no events found for aggregate with this id</exception>
        /// <param name="id">The aggregate guid</param>
        /// <returns></returns>
        T Get<T>(Guid id) where T : class,IAggregate;
        /// <summary>
        /// Replay all the events up to date and push through event bus
        /// </summary>
        /// <param name="date"></param>
        void Replay(Instant date);
    }
}