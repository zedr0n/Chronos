using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public class ProjectionUpdated<T> : EventBase
    {
        public Instant AsOf { get; set; }
    }
}