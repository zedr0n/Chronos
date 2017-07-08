using System;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public class Projection : IProjection
    {
        public Instant AsOf { get; set; } = Instant.MaxValue;
        public int LastEvent { get; set; } = -1;
        public T Copy<T>() where T : IProjection {
            return (T) MemberwiseClone();
        }
    }
}