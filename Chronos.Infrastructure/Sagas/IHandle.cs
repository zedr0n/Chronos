using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Sagas
{
    public interface IHandle<TEvent>
        where TEvent : IEvent
    {
        void When(TEvent e);
    }
}