using System;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Infrastructure
{
    public static class StreamExtensions
    {
        public static StreamDetails StreamDetails(this IAggregate aggregate)
        {
            return new StreamDetails
            {
                Name = $"{aggregate.GetType().Name}-{aggregate.Id}",
                SourceType = aggregate.GetType()
            };
        }

        public static StreamDetails StreamDetails(this ISaga saga)
        {
            return new StreamDetails
            {
                Name = $"{saga.GetType().Name}-{saga.SagaId}",
                SourceType = saga.GetType()
            };
        }

        public static string StreamName(this IAggregate aggregate)
        {
            return $"{aggregate.GetType().Name}-{aggregate.Id}";
        }

        public static string StreamName<TKey, T>(TKey key)
        {
            return $"{typeof(T).Name}-{key}";
        }

        public static string StreamName<T>(Guid id)
        {
            return $"{typeof(T).Name}-{id}";
        }
    }
}