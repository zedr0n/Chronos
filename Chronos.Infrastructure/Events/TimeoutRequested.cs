using System;

namespace Chronos.Infrastructure.Events
{
    public class TimeoutRequested : EventBase
    {
        public Guid ScheduleId { get; set; }    
    }
}