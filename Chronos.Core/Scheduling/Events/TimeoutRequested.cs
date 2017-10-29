using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Scheduling.Events
{
    public class TimeoutRequested : EventBase
    {
        public Guid ScheduleId { get; set; }    
    }
}