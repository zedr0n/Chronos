using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Common.Events
{
    public class StopCompleted : EventBase
    {
        public Guid ScheduleId { get; set; }
    }
}