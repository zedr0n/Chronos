using NodaTime;

namespace Chronos.Infrastructure
{
    public interface IReplayStrategy
    {
        void Replay(Instant date);
    }
}