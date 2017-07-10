using System;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Events
{
    public interface IEvent : IMessage
    {
        Guid SourceId { get; }
        Instant Timestamp { get; set; }
        int EventNumber { get; set; }
        bool Replaying { get; set; }
    }
}