using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Scheduling.Events
{
    public class TimeoutCompleted : EventBase
    {
        public Guid ScheduleId { get; set; }
    }
}