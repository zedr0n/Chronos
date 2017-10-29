using System;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Events
{
    public class CommandSchedulingRequested : EventBase
    {
        public Guid ScheduleId { get; set; }
        public ICommand Command { get; set; }
        public Instant When { get; set; }
    }
}