using System;
using NodaTime;

namespace Chronos.Infrastructure.Interfaces
{
    public interface IEvent : IMessage
    {
        Guid SourceId { get; }
        Instant Timestamp { get; set; }
        int EventNumber { get; set; }
    }
}