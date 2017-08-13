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