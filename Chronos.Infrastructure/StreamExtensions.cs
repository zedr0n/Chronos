using System;

namespace Chronos.Infrastructure
{
    public static class StreamExtensions
    {
        public static StreamDetails StreamDetails(this IAggregate aggregate)
        {
            return new StreamDetails(name: $"{aggregate.GetType().Name}-{aggregate.Id}", sourceType: aggregate.GetType());
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