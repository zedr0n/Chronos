using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure.Misc;

namespace Chronos.Infrastructure.Projections
{
    public class ProjectionRepository : IProjectionRepository
    {
        private readonly Dictionary<Type,List<IProjection>> _dictionary = new Dictionary<Type, List<IProjection>>();
        public IEnumerable<T> Find<T>(Func<T, bool> criteria) where T : class, IProjection
        {
            if (!_dictionary.TryGetValue(typeof(T), out var projections) || !projections.Any())
                return null;

            var satifyingProjections = projections.Where(x => criteria(x as T)).Cast<T>().AsCachedAnyEnumerable();
            return satifyingProjections.Any() ? satifyingProjections : null;
        }

        public IEnumerable<T> Get<T>(Func<T, bool> criteria) where T : class, IProjection
        {
            var projections = Find(criteria);
            if (projections == null)
                throw new InvalidOperationException("No matching projections found");

            return projections;
        }

        public void Add<T>(T projection) where T : IProjection
        {
            if(!_dictionary.TryGetValue(typeof(T),out var projections ))
                _dictionary.Add(typeof(T),new List<IProjection> {projection});
            else
                projections.Add(projection);
        }
    }
}