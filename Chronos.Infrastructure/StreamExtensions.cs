using System;

namespace Chronos.Infrastructure
{
    public static class StreamExtensions
    {
        public static string StreamName(this IAggregate aggregate)
        {
            return $"{aggregate.GetType().Name}-{aggregate.Id}";
        }

        public static string StreamName<T>(Guid id)
        {
            return $"{typeof(T).Name}-{id}";
        }
    }
}