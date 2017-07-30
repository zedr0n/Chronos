using System;

namespace Chronos.Infrastructure.Events
{
    public class TimeoutCompleted : EventBase
    {
        public Guid ScheduleId { get; set; }
    }
}