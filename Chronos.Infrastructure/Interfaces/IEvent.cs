using NodaTime;

namespace Chronos.Infrastructure.Interfaces
{
    public interface IEvent : IMessage
    {
        bool Insertable { get; }
        
        Instant Timestamp { get; set; }
        int EventNumber { get; set; }
        int Version { get; set; }
        
        double Hash { get; set; }
    }
}