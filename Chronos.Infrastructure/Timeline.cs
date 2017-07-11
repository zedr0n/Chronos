using System;
using NodaTime;

namespace Chronos.Infrastructure
{
    public class Timeline : ITimeline
    {
        private readonly IClock _clock;
        private Instant _current;

        public Timeline(IClock clock)
        {
            _clock = clock;
        }

        public bool Live { get; private set; } = true;

        public void Set(Instant date)
        {
            _current = _clock.GetCurrentInstant();
            if(_current.CompareTo(date) <= 0)
                throw new InvalidOperationException("Cannot time travel to the future");

            Live = false;
            _current = date;
        }

        public void Reset()
        {
            Live = true;
            _current = _clock.GetCurrentInstant();
        }

        public Instant Now()
        {
            if (Live)
                _current = _clock.GetCurrentInstant();
            return _current;
        }
    }
}