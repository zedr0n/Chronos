using System;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public interface IProjectionManager
    {
        /// <summary>
        /// Add a time point for projection observation
        /// </summary>
        /// <typeparam name="T">Projection type</typeparam>
        /// <param name="criteria">Projection criteria</param>
        /// <param name="instant">Projection will contain all events up to this moment in time</param>
        void RegisterJuncture<T>(Func<T, bool> criteria, Instant instant)
            where T : class, IProjection;

        /// <summary>
        /// Rebuild all the projections matching criteria
        /// </summary>
        /// <typeparam name="T">Projection type</typeparam>
        /// <param name="criteria">Filtering criteria</param>
        void Rebuild<T>(Func<T, bool> criteria)
            where T : class, IProjection;

        void Rebuild<T>(Func<T, bool> criteria,Func<IEvent,bool> eventCriteria)
            where T : class, IProjection;
    }
}