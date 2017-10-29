using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Common.Events
{
    public class TimeoutCanceled : EventBase
    {
        public Guid ScheduleId { get; set; }
    }
}