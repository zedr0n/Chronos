using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Infrastructure
{
    public static class AggregateExtensions
    {
        public static int ExpectedVersion(this IAggregate aggregate, IEnumerable<IEvent> events)
        {
            return aggregate.Version - events.Count();
        }
    }

    public abstract class DomainRepositoryBase : IDomainRepository
    {
        private readonly HashSet<Guid> _existingAggregates = new HashSet<Guid>();
        protected readonly Dictionary<Type, IAggregate> LastAggregates = new Dictionary<Type, IAggregate>();

        public abstract void Save<T>(T aggregate) where T : IAggregate;
        public bool Exists<T>(Guid id) where T : IAggregate
        {
            if (LastAggregates.ContainsKey(typeof(T)) && LastAggregates[typeof(T)].Id == id || _existingAggregates.Contains(id))
            {
                _existingAggregates.Add(id);
                return true;
            }
            return Find<T>(id) != null;
        }

        public abstract T Find<T>(Guid id) where T : IAggregate;
        public T Get<T>(Guid id) where T : IAggregate
        {
            var entity = Find<T>(id);
            if (entity == null)
                throw new InvalidOperationException("No events recorded for aggregate");

            return entity;
        }

        public abstract void Replay(Instant date);

        protected static int ExpectedVersion<T>(IAggregate aggregate, List<T> events)
        {
            var expectedVersion = aggregate.Version - events.Count;

            return expectedVersion;
        }
    }
}