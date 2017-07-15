using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public class CommandBase : ICommand
    {
        public Guid AggregateId { get; set; }
    }
}