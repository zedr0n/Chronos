using System;
using Chronos.Infrastructure.Sagas;
using NodaTime;

namespace Chronos.Infrastructure
{
    public interface ISagaRepository
    {
        /// <summary>
        /// Saves the saga events to event store and publish those to event bus
        /// </summary>
        /// <param name="aggregate">The saga instance</param>
        void Save<T>(T saga) where T : ISaga;
        /// <summary>
        /// Rebuild the aggregate from event history extracted from Event Store
        /// </summary>
        /// <param name="id">The aggregate guid</param>
        /// <typeparam name="T">Aggregate type</typeparam>
        /// <returns>Aggregate or null if no events found</returns>
        T Find<T>(Guid id) where T : ISaga;
        /// <summary>
        /// Rebuilds the aggregate from event history extracted from Event Store
        /// </summary>
        /// <exception cref="ArgumentException">if no events found for aggregate with this id</exception>
        /// <param name="id">The aggregate guid</param>
        /// <returns></returns>
        T Get<T>(Guid id) where T : ISaga;
    }
}