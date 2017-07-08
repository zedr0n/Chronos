using System;
using System.Collections.Generic;
using System.Linq;

namespace Chronos.Infrastructure.Projections
{
    public class ProjectionRepository : IProjectionRepository
    {
        private readonly Dictionary<Type,List<object>> _dictionary = new Dictionary<Type, List<object>>();
        public IEnumerable<T> Find<T>(Func<T, bool> criteria) where T : class, IProjection
        {
            if (!_dictionary.TryGetValue(typeof(T), out var projections))
                return null;
            
            return projections.Where(x => criteria(x as T)).Cast<T>();
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
                _dictionary.Add(typeof(T),new List<object> {projection});
            else
                projections.Add(projection);
        }
    }
}