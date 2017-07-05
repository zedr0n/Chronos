using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{

    public class Repository<T> : IRepository<T> where T : class, IProjection
    {
        private readonly Dictionary<Guid, List<T>> _dictionary = new Dictionary<Guid, List<T>>();

        public IEnumerable<T> All()
        {
            return _dictionary.Values.SelectMany(x => x);
        }

        public T Get(Guid guid)
        {
            if(!_dictionary.TryGetValue(guid, out List<T> value) || value.Count == 0)
                throw new ArgumentException("Object not found in repository");
            return value.SingleOrDefault();
        }

        public IEnumerable<T> Get(Guid guid, Instant asOf)
        {
            return Get(guid, x => x.AsOf.CompareTo(asOf) <= 0);
        }

        public IEnumerable<T> Get(Guid guid, Func<T, bool> criteria)
        {
            var projections = Find(guid, criteria);
            if (projections == null)
                throw new InvalidOperationException("No projections satisfying the criteria found");
            return projections;
        }

        public void Add(Guid guid, T projection)
        {
            Add(guid, projection, Instant.MaxValue);
        }

        public bool Add(Guid guid, T projection, Instant asOf)
        {
            var updated = false;
            if (!_dictionary.TryGetValue(guid, out List<T> projections))
            {
                projection.AsOf = asOf;
                _dictionary.Add(guid, new List<T> {projection});
            }
            else
            {
                var existingProjection = projections.SingleOrDefault(x => x.AsOf.CompareTo(asOf) == 0);

                if (existingProjection != null)
                {
                    projections.Remove(existingProjection);
                    updated = true;
                }

                projection.AsOf = asOf;
                projections.Add(projection);
            }

            return !updated;
        }

        public IEnumerable<T> Find(Guid guid, Func<T, bool> criteria)
        {
            _dictionary.TryGetValue(guid, out List<T> projections);
            var satisfyingProjections = projections?.Where(criteria).OrderBy(x => x.AsOf);
            return !satisfyingProjections.Any() ? null : satisfyingProjections;
        }

        public IEnumerable<T> Find(Guid guid, Instant asOf)
        {
            return Find(guid, x => x.AsOf.CompareTo(asOf) <= 0);
        }

        public Guid ResolveId(T instance)
        {
            return _dictionary.Single(x => x.Value.Contains(instance)).Key;
        }
    }
}