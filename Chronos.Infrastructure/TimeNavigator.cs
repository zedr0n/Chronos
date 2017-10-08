using System;
using Chronos.Infrastructure.Logging;
using NodaTime;
using NodaTime.Text;

namespace Chronos.Infrastructure
{
    public class TimeNavigator : ITimeNavigator
    {
        private readonly IReplayStrategy _replayStrategy;
        private readonly ITimeline _timeline;
        private readonly IDebugLog _debugLog;

        public TimeNavigator(ITimeline timeline, IDebugLog debugLog, IReplayStrategy replayStrategy)
        {
            _timeline = timeline;
            _debugLog = debugLog;
            _replayStrategy = replayStrategy;
        }

        private void Replay(Instant date)
        {
            _replayStrategy.Replay(date);
            //_domainRepository.Reset(date);
        }

        public void GoTo(Instant date)
        {
            _debugLog.WriteLine("Warping to " + InstantPattern.ExtendedIso.Format(date));
            //_timerService.Reset();
            _timeline.Alternate(Guid.NewGuid());
            _timeline.Set(date);
            Replay(date);
        }

        public void Reset()
        {
            //_timerService.Reset();
            _timeline.Reset();
            var now = _timeline.Now();
            _debugLog.WriteLine("Resetting time stream, currently at " + InstantPattern.ExtendedIso.Format(now));
            Replay(now);
        }

        public void Advance(Duration duration)
        {
            if (_timeline.Live)
                throw new InvalidOperationException("Time cannot be advance when not in historical state");

            var date = _timeline.Now().Plus(duration);
            //GoTo(date);
            _timeline.Set(date);
            Replay(date);
        }
    }
}