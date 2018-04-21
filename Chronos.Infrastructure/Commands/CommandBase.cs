using System;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Commands
{
    public class CommandBase : ICommand
    {
        /// <summary>
        /// Command target id
        /// </summary>
        public virtual Guid TargetId { get; set; }
        public Instant Timestamp { get; set; }
    }
}