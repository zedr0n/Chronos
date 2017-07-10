using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Infrastructure
{
    public interface ITimerService : IConsumer<TimeoutRequested>
    {
        void Reset();
    }
}