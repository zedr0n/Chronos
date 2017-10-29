using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public interface IEventBus
    {
        void Alert(IEvent e);
    }
}