using System.IO;
using Chronos.Infrastructure.Interfaces;
using Chronos.Persistence.Types;

namespace Chronos.Persistence.Serialization
{
    public class EventSerializer : IEventSerializer
    {
        private readonly ISerializer _serializer;

        public EventSerializer(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public Event Serialize(IEvent e)
        {
            Event serialized;

            using (var writer = new StringWriter())
            {
                _serializer.Serialize(writer, e);

                serialized = new Event
                {
                    Guid = e.SourceId,
                    TimestampUtc = e.Timestamp.ToDateTimeUtc(),
                    Payload = writer.ToString()
                };
            }

            return serialized;
        }
        public IEvent Deserialize(Event e)
        {
            using (var reader = new StringReader(e.Payload))
            {
                var @event = _serializer.Deserialize<IEvent>(reader);
                @event.EventNumber = e.EventNumber;
                return @event;
            }
        }
    }
}