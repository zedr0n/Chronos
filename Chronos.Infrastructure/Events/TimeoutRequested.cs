using System;
using NodaTime;

namespace Chronos.Infrastructure.Events
{
    public class TimeoutRequested : EventBase
    {
        public Guid ScheduleId { get; set; }
        public Instant When { get; set; }
    }
}