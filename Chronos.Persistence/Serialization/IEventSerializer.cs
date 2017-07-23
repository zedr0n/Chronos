using Chronos.Infrastructure.Interfaces;
using Chronos.Persistence.Types;

namespace Chronos.Persistence.Serialization
{
    public interface IEventSerializer
    {
        Event Serialize(IEvent e);

        IEvent Deserialize(Event e);

    }
}