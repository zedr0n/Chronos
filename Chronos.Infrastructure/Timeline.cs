using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

            Reset();
        }
        
        public IObservable<T> StopAt<T>(Instant date, T value)
        {
            return Observable.Create((IObserver<T> observer) =>
            {
                var subscription = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(10))
                    .Subscribe(l =>
                    {
                        if (Now() < date) 
                            return;
                        observer.OnNext(value);
                        observer.OnCompleted();
                    });
                
                return Disposable.Create(() => subscription.Dispose());
            });
        }

        private Guid _timelineId = Guid.NewGuid();
        
        public Guid TimelineId => Live ? Guid.Empty : _timelineId;
        public bool Live { get; private set; } = true;

        public void Alternate(Guid timelineId)
        {
            _timelineId = timelineId;
        }
        
        public void Set(Instant date)
        {
            _current = _clock.GetCurrentInstant();
            if(_current.CompareTo(date) <= 0)
                throw new InvalidOperationException("Cannot time travel to the future");

            Live = false;
            _current = date;
            
            if(_timelineId == Guid.Empty)
                throw new InvalidOperationException("Alternative timeline detected but not initialized");
        }

        public void Reset()
        {
            Live = true;
            _timelineId = Guid.Empty;
            
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