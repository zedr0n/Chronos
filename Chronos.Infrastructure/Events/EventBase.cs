using System;
using NodaTime;

namespace Chronos.Infrastructure.Events
{
    public abstract class EventBase : IEvent
    {
        public Guid SourceId { get; set; }
        public Instant Timestamp { get; set; } = SystemClock.Instance.GetCurrentInstant();
        public int EventNumber { get; set; } = -1;
    }
}