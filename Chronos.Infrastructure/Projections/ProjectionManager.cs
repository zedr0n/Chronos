using System;
using System.Linq;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using NodaTime;
using NodaTime.Text;

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
            var events = _connection.GetAggregateEvents().Where(eventCriteria).OrderBy(e => e.Timestamp);
            var projections = _projectionRepository.Find(criteria)?.ToList();
            if (projections == null)
                return;

            foreach (var projection in projections)
                projection.LastEvent = -1;

            var projector = _projectorRepository.Get<IProjector<T>>();

            _debugLog.WriteLine("@ProjectionManager : ");
            foreach (var e in events)
            {
                _debugLog.WriteLine("    " + e.GetType().Name + "[R]( " + InstantPattern.ExtendedIso.Format(e.Timestamp) + " )");
                if(projector.Dispatch(e))
                    _debugLog.WriteLine("     -> " + projector.GetType().Name);
                //_debugLog.WriteLine("    " + e.GetType().Name + "[R]( " + InstantPattern.ExtendedIso.Format(e.Timestamp) + " )");
            }
        }

        public void RegisterJuncture<T>(Func<T, bool> criteria, Instant instant)
            where T : class, IProjection
        {
            var projections = _projectionRepository.Find(criteria)?.ToList();
            if(projections == null)
                throw new InvalidOperationException("Impossible to build projection with such criteria");
            if (projections.Any(x => x.AsOf.CompareTo(instant) == 0))
                return;

            var projection = projections.First().Copy<T>();
            projection.AsOf = instant;
            _projectionRepository.Add(projection);

            _debugLog.WriteLine("Project " + typeof(T).Name + " as of " + InstantPattern.ExtendedIso.Format(instant));
            Rebuild<T>(x => criteria(x) && x.AsOf.CompareTo(instant) == 0,e => e.Timestamp.CompareTo(instant) <= 0);
        }
    }
}