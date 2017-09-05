using System;

namespace Chronos.Infrastructure
{
    public static class StreamExtensions
    {
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
        
        public static int HashString(this string text)
        {
            unchecked
            {
                int hash = 23;
                foreach (char c in text)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }
    }
}