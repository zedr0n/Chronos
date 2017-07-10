using NodaTime;

namespace Chronos.Infrastructure.Events
{
    public class TimeoutRequested : EventBase
    {
        public Instant When { get; set; }
    }
}