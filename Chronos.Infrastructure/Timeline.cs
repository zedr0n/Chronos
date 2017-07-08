using System;
using NodaTime;

namespace Chronos.Infrastructure
{
    public class Timeline : ITimeline
    {
        private readonly IClock _clock = SystemClock.Instance;
        private Instant _current;
        public bool Live { get; private set; } = true;

        public void Set(Instant date)
        {
            if(!Live)
                throw new InvalidOperationException("Timeline is already at historical value");

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
            return Live ? _clock.GetCurrentInstant() : _current;
        }
    }
}