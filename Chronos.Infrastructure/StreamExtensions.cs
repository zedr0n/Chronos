using System;

namespace Chronos.Infrastructure
{
    public static class StreamExtensions
    {
        public static string AggregateToStreamName(Type type, Guid id)
        {
            return $"{type.Name}-{id}";
        }
    }
}