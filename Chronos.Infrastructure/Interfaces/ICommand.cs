using System;
using NodaTime;

namespace Chronos.Infrastructure.Interfaces
{
    public interface ICommand : IMessage
    {
        Guid TargetId { get; set; }
        Instant Timestamp { get; set; }
    }
}