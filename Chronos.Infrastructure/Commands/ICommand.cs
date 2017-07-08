using System;

namespace Chronos.Infrastructure.Commands
{
    public interface ICommand
    {
        Guid AggregateId { get; set; }
    }
}