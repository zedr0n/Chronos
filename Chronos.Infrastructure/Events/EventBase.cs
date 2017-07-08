using System;
using NodaTime;

namespace Chronos.Infrastructure.Events
{
    public class EventBase : IEvent
    {
        public Guid SourceId { get; set; }
        public Instant Timestamp { get; set; }
        public int EventNumber { get; set; } = -1;
    }
}