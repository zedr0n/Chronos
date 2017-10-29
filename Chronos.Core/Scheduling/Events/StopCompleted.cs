using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Scheduling.Events
{
    public class StopCompleted : EventBase
    {
        public Guid ScheduleId { get; set; }
    }
}