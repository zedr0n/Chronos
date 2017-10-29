using System;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Core.Common.Commands
{
    public class ScheduleCommand : CommandBase
    {
        public Guid ScheduleId { get; set; }
        public ICommand Command { get; set; }
        public Instant Date { get; set; }
    }
}