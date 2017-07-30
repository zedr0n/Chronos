using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Events
{
    public class EventBase : IEvent
    {
        private readonly EventInfo _eventInfo = new EventInfo();

        private class EventInfo
        {
            internal Instant Timestamp { get; set; }
            internal int EventNumber { get; set; } = -1;
        }

        public Instant Timestamp
        {
            set => _eventInfo.Timestamp = value;
            get => _eventInfo.Timestamp;
        }

        public int EventNumber
        {
            set => _eventInfo.EventNumber = value;
            get => _eventInfo.EventNumber;
        }
    }

}