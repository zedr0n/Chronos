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
            //if(!Live)
            //    throw new InvalidOperationException("Timeline is already at historical value");

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
            if (!Live)
                return _current;
            var now = _clock.GetCurrentInstant();
            // if we process events faster than clock ticks
            // manually increment the time
            if (now.CompareTo(_current) <= 0)
                _current = _current.PlusNanoseconds(1);
            else
                _current = now;

            return _current;
        }
    }
}