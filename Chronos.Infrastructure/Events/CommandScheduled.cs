using System;
using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Infrastructure.Events
{
    public class CommandScheduled : EventBase
    {
        public Guid ScheduleId { get; set; }
        public ICommand Command { get; set; }
        public Instant Time { get; set; }
    }
}