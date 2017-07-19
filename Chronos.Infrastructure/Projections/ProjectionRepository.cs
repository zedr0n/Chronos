using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure.Misc;
using NodaTime;

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

        public T Find<TKey, T>(TKey key) where T : class, IProjection<TKey>
                                         where TKey : IEquatable<TKey>
        {
            if (!_dictionary.TryGetValue(typeof(T), out var projections) || !projections.Any())
                return null;

            var projection = projections.Cast<T>()?.SingleOrDefault(p => p.Key.Equals(key));
            return projection;
        }

        public T Find<TKey, T>(HistoricalKey<TKey> key) where TKey : IEquatable<TKey> where T : class, IProjection<TKey>, new()
        {
            return key.AsOf == Instant.MaxValue ? 
                Find<TKey, T>(key.Key) : Find<HistoricalKey<TKey>, HistoricalProjection<TKey,T>>(key)?.Projection;
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