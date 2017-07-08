using System;
using NodaTime;

namespace Chronos.Infrastructure.Events
{
    public interface IEvent
    {
        Guid SourceId { get; }
        Instant Timestamp { get; set; }
        int EventNumber { get; set; }        
    }
}