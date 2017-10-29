using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Scheduling.Events
{
    public class StopRequested : EventBase
    {
        public Guid ScheduleId { get; set; }
    }
}