using System;
using Chronos.Core.Common.Events;
using Chronos.Core.Scheduling.Events;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Core.Scheduling
{
    public interface IScheduler
    {
        IObservable<TimeoutCompleted> Timeout(Duration duration);
        IObservable<TimeoutCompleted> StopAt(Instant date);

        IDisposable ScheduleTimeout(Guid scheduleId, Duration duration,
            Action<TimeoutCompleted> action);

        IDisposable ScheduleStop(Guid scheduleId, Instant when,
            Action<TimeoutCompleted> action);

        void Cancel(Guid scheduleId);
    }
}