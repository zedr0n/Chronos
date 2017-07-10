using System;
using System.Linq;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
 
    public class ProjectionManager : IProjectionManager
    {
        private readonly IEventStoreConnection _connection;
        private readonly IProjectionRepository _projectionRepository;
        private readonly IProjectorRepository _projectorRepository;
        private readonly IDebugLog _debugLog;

        public ProjectionManager(IEventStoreConnection connection, IProjectionRepository projectionRepository, IProjectorRepository projectorRepository, IDebugLog debugLog)
        {
            _connection = connection;
            _projectionRepository = projectionRepository;
            _projectorRepository = projectorRepository;
            _debugLog = debugLog;
        }

        public void Rebuild<T>(Func<T, bool> criteria) where T : class, IProjection
        {
            Rebuild(criteria,e => true);
        }

        public void Rebuild<T>(Func<T, bool> criteria, Func<IEvent, bool> eventCriteria)
            where T : class, IProjection
        {
            var events = _connection.GetAllEvents().Where(eventCriteria).OrderBy(e => e.Timestamp);
            var projections = _projectionRepository.Find(criteria)?.ToList();
            if (projections == null)
                return;

            foreach (var projection in projections)
                projection.LastEvent = -1;

            var projector = _projectorRepository.Get(typeof(T));

            foreach (var e in events)
            {
                if(projector.Dispatch(e))
                    _debugLog.WriteLine("Rebuilding projection " + typeof(T).Name + " : " + e.GetType().Name);
            }
        }

        public void RegisterJuncture<T>(Func<T, bool> criteria, Instant time)
            where T : class, IProjection
        {
            var projections = _projectionRepository.Find(criteria)?.ToList();
            if(projections == null)
                throw new InvalidOperationException("Impossible to build projection with such criteria");
            if (projections.Any(x => x.AsOf.CompareTo(time) == 0))
                return;

            var projection = projections.First().Copy<T>();
            projection.AsOf = time;
            _projectionRepository.Add(projection);

            Rebuild<T>(x => criteria(x) && x.AsOf.CompareTo(time) == 0,e => e.Timestamp.CompareTo(time) <= 0);
        }
    }
}