using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public class CommandBase : ICommand
    {
        public virtual Guid TargetId { get; set; }
    }
}