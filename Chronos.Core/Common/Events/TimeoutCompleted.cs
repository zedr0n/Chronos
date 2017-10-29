using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Common.Events
{
    public class TimeoutCompleted : EventBase
    {
        public Guid ScheduleId { get; set; }
    }
}