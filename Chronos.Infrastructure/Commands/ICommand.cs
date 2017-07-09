using System;

namespace Chronos.Infrastructure.Commands
{
    public interface ICommand : IMessage
    {
        Guid AggregateId { get; set; }
    }
}