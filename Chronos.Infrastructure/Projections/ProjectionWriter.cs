using System;
using System.Linq;

namespace Chronos.Infrastructure.Projections
{
    public interface IProjectionWriter
    {
        void UpdateOrThrow<TKey, TProjection>(TKey key, Action<TProjection> action)
            where TProjection : class, IProjection<TKey>
            where TKey : IEquatable<TKey>;

        void Add<TProjection>(TProjection projection)
            where TProjection : class, IProjection;
    }

    public class ProjectionWriter : IProjectionWriter
    {
        private readonly IProjectionRepository _repository;

        public ProjectionWriter(IProjectionRepository repository)
        {
            _repository = repository;
        }

        public void UpdateOrThrow<TKey, TProjection>(TKey key, Action<TProjection> action)
            where TProjection : class, IProjection<TKey>
            where TKey : IEquatable<TKey>
        {
            var projection = _repository.Find<TKey,TProjection>(key);
            if (projection == null)
                return;

            action(projection);
        }

        public void Add<TProjection>(TProjection projection)
            where TProjection : class, IProjection
        {
            _repository.Add(projection);
        }
    }
}