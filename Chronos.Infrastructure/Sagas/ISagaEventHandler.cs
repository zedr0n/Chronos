using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Sagas
{
    public interface ISagaEventHandler
    {
        void Handler<TEvent, T>(T saga, TEvent e) where TEvent : class, IEvent
            where T : class, ISaga;
    }
}