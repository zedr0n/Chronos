using NodaTime;

namespace Chronos.Infrastructure.Interfaces
{
    public interface IEvent : IMessage
    {
        Instant Timestamp { get; set; }
        int EventNumber { get; set; }
    }
}