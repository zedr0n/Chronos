using System;

namespace Chronos.Infrastructure.Commands
{
    public class CommandBase : ICommand
    {
        public Guid AggregateId { get; set; }
    }
}