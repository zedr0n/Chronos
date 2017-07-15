using System;

namespace Chronos.Infrastructure.Interfaces
{
    public interface ICommand : IMessage
    {
        Guid AggregateId { get; set; }
    }
}