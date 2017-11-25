using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Security.Cryptography.X509Certificates;
using Chronos.Core.Common.Events;
using Chronos.Core.Scheduling.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Logging;
using NodaTime;

namespace Chronos.Core.Scheduling
{
    public class Scheduler : IScheduler
    {
        private readonly ITimeline _timeline;
        private readonly IDebugLog _debugLog;
        
        private readonly ConcurrentDictionary<Guid,Lazy<IDisposable>> _subscriptions = new ConcurrentDictionary<Guid, Lazy<IDisposable>>();

        public Scheduler(ITimeline timeline, IDebugLog debugLog)
        {
            _timeline = timeline;
            _debugLog = debugLog;
        }

        private IDisposable CreateSubscription<T>(IObservable<T> observable,Guid scheduleId, Action<T> action)
        {
            _debugLog.WriteLine("Requesting timeout " + scheduleId);
            return observable.Finally(() =>
                {
                    _debugLog.WriteLine("Subscription completed " + scheduleId);
                    _subscriptions.TryRemove(scheduleId, out var _);
                })
                //.ObserveOn(System.Reactive.Concurrency.Scheduler.Default)
                .Subscribe(action);
        }

        public IDisposable ScheduleTimeout(Guid scheduleId, Duration duration,
            Action<TimeoutCompleted> action)
        {
            var sub = _subscriptions.GetOrAdd(scheduleId, x => new Lazy<IDisposable>(
                () => CreateSubscription(Timeout(duration).Do(e => e.ScheduleId = scheduleId), x, action)));

            return sub.Value;
        }

        public void Cancel(Guid scheduleId)
        {
            if (_subscriptions.TryRemove(scheduleId, out var subscription))
                subscription.Value.Dispose();
        }
        
        public IObservable<TimeoutCompleted> Timeout(Duration duration)
        {
            var observable = Observable.Timer(TimeSpan.FromTicks(duration.BclCompatibleTicks))
                .Select(x => new TimeoutCompleted
                {
                    Timestamp = _timeline.Now()
                })
                .Do(e => _debugLog.WriteLine("Timeout completed"));
            return observable;
        }

        public IDisposable ScheduleStop(Guid scheduleId, Instant date,
            Action<TimeoutCompleted> action)
        {
            var sub = _subscriptions.GetOrAdd(scheduleId, x => new Lazy<IDisposable>(
                () => CreateSubscription(
                    StopAt(date).Do(e => e.ScheduleId = scheduleId), scheduleId, action)));

            return sub.Value;
        }
        
        public IObservable<TimeoutCompleted> StopAt(Instant date)
        {
            var observable = Observable.Interval(TimeSpan.FromMilliseconds(100))
                .SkipWhile(x => _timeline.Now() < date)
                .Take(1)
                .Select(x => new TimeoutCompleted
                {
                    Timestamp = _timeline.Now()
                });
            return observable;
        }
    }
}