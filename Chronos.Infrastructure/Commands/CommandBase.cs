using System;
using NodaTime;

namespace Chronos.Infrastructure.Commands
{
    public class CommandBase : ICommand
    {
        public Guid AggregateId { get; set; }
        public Instant TimestampOverride { get; set; } = Instant.MaxValue;
    }
}