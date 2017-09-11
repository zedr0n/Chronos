using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public interface IAggregate : IConsumer
    {
        /// <summary>
        /// Unique guid key of the aggregate
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// Aggregate version ( for optmistic concurrency )
        /// </summary>
        int Version { get; }
        IEnumerable<IEvent> UncommitedEvents { get; }
        
        void ClearUncommitedEvents();

        /// <summary>
        /// Hydrate the aggregate from event sequence
        /// </summary>
        /// <param name="id">Aggregate id</param>
        /// <param name="pastEvents">Past event sequence</param>
        /// <typeparam name="T">Aggregate type</typeparam>
        /// <returns>Hydrated aggregate instance</returns>
        T LoadFrom<T>(Guid id, IEnumerable<IEvent> pastEvents) where T : class, IAggregate;

    }
}