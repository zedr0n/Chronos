using System;
using NodaTime;

namespace Chronos.Infrastructure.Commands
{
    public interface ICommand
    {
        Guid AggregateId { get; set; }
        Instant TimestampOverride { get; set; }
    }
}