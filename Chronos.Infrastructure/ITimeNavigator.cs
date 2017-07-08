using NodaTime;

namespace Chronos.Infrastructure
{
    public interface ITimeNavigator
    {
        void GoTo(Instant date);
        void Reset();
        void Advance(Duration duration);

    }
}