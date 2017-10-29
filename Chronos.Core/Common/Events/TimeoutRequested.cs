using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Common.Events
{
    public class TimeoutRequested : EventBase
    {
        public Guid ScheduleId { get; set; }    
    }
}