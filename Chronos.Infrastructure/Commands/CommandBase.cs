using System;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Commands
{
    public class CommandBase : ICommand
    {
        public virtual Guid TargetId { get; set; }
        public Instant Timestamp { get; set; }
    }
}