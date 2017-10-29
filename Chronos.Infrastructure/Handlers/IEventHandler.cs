using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Handlers
{
    public interface IEventHandler<T> where T : IEvent
    {
        void Handle(T @event);
    }
}