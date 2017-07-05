using System;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public class ProjectionWriter<TProjection> : IProjectionWriter<TProjection>
        where TProjection : class,IProjection
    {
        private readonly IRepository<TProjection> _repository;
        private readonly IEventBus _eventBus;

        public ProjectionWriter(IRepository<TProjection> repository, IEventBus eventBus)
        {
            _repository = repository;
            _eventBus = eventBus;
        }

        public void Add(Guid id, TProjection projection) => Add(id,projection,Instant.MaxValue);
        public void Add(Guid id, TProjection projection, Instant asOf)
        {
            if(_repository.Add(id, projection, asOf))
                _eventBus.Publish(new ProjectionCreated<TProjection> { SourceId = id, AsOf = asOf });
            else
                _eventBus.Publish(new ProjectionUpdated<TProjection> { SourceId = id, AsOf = asOf});        
        }

        public void UpdateOrThrow(Guid id, Action<TProjection> action, Instant asOf)
        {
            var projections = _repository.Get(id, p => p.AsOf.CompareTo(asOf) >= 0);

            foreach (var projection in projections)
            {
                action(projection);
                _eventBus.Publish(new ProjectionUpdated<TProjection> { SourceId = id, AsOf = projection.AsOf});
            }
        }
    }
}