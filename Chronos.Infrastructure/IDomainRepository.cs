using System;
using Chronos.Infrastructure.Aggregates;

namespace Chronos.Infrastructure
{
    public interface IDomainRepository
    {
        /// <summary>
        /// Saves the aggregate events to event store and publish those to event bus
        /// </summary>
        /// <param name="aggregate">The aggregate instance</param>
        void Save<T>(T aggregate) where T : IAggregate;
        /// <summary>
        /// Rebuild the aggregate from event history extracted from Event Store
        /// </summary>
        /// <param name="id">The aggregate guid</param>
        /// <typeparam name="T">Aggregate type</typeparam>
        /// <returns>Aggregate or null if no events found</returns>
        T Find<T>(Guid id) where T : IAggregate;
        /// <summary>
        /// Rebuilds the aggregate from event history extracted from Event Store
        /// </summary>
        /// <exception cref="ArgumentException">if no events found for aggregate with this id</exception>
        /// <param name="id">The aggregate guid</param>
        /// <returns></returns>
        T Get<T>(Guid id) where T : IAggregate;

    }
}