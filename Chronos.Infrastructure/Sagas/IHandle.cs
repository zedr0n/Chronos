using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Sagas
{
    public interface IHandle<in TEvent>
        where TEvent : IEvent
    {
        void When(TEvent e);
    }
}