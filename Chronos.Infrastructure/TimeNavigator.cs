using System;
using NodaTime;

namespace Chronos.Infrastructure
{
    public class TimeNavigator : ITimeNavigator
    {
        private readonly IDomainRepository _domainRepository;
        private readonly ITimeline _timeline;

        public TimeNavigator(ITimeline timeline, IDomainRepository domainRepository)
        {
            _timeline = timeline;
            _domainRepository = domainRepository;
        }

        private void Replay(Instant date)
        {
            _domainRepository.Replay(date);
        }

        public void GoTo(Instant date)
        {
            _timeline.Set(date);
            Replay(date);
        }

        public void Reset()
        {
            _timeline.Reset();
            Replay(_timeline.Now());
        }

        public void Advance(Duration duration)
        {
            if (_timeline.Live)
                throw new InvalidOperationException("Time cannot be advance when not in historical state");

            var date = _timeline.Now().Plus(duration);
            GoTo(date);
        }
    }
}