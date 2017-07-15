using System;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Events
{
    public class EventBase : IEvent
    {
        public Guid SourceId { get; set; }
        public Instant Timestamp { get; set; }
        public int EventNumber { get; set; } = -1;
        public bool Replaying { get; set; } = false;
    }
}