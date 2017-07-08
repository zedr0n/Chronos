using System;
using System.Collections.Generic;

namespace Chronos.Infrastructure
{
    public abstract class DomainRepositoryBase : IDomainRepository
    {
        public abstract void Save<T>(T aggregate) where T : IAggregate;
        public abstract T Find<T>(Guid id) where T : IAggregate;
        public T Get<T>(Guid id) where T : IAggregate
        {
            var entity = Find<T>(id);
            if (entity == null)
                throw new InvalidOperationException("No events recorded for aggregate");

            return entity;
        }

        protected static int CalculateExpectedVersion<T>(IAggregate aggregate, List<T> events)
        {
            var expectedVersion = aggregate.Version - events.Count;

            return expectedVersion;
        }
    }
}