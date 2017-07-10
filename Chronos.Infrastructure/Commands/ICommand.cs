using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public interface ICommand : IMessage
    {
        Guid AggregateId { get; set; }
    }
}