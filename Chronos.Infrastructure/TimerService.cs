using System;
using System.Collections.Generic;
using System.Threading;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure
{
    public class TimerService : ITimerService
    {
        private readonly IEventBus _eventBus;
        private readonly ITimeline _timeline;
        private readonly Dictionary<Guid,Timer> _timers = new Dictionary<Guid, Timer>();
        private const int PollingFrequency = 100;

        public TimerService(IEventBus eventBus, ITimeline timeline)
        {
            _eventBus = eventBus;
            _timeline = timeline;
            this.RegisterAll(eventBus);
        }

        private void CheckTimeout(TimeoutRequested e)
        {
            if (_timeline.Now().CompareTo(e.When) < 0)
                return;
            lock (_timers)
            {
                if (!_timers.ContainsKey(e.SourceId))
                    return;

                _timers[e.SourceId].Dispose();
                _timers.Remove(e.SourceId);
                _eventBus.Publish(new TimeoutCompleted { SourceId = e.SourceId });
            }
        }

        public void When(TimeoutRequested e)
        {
            lock (_timers)
            {
                if (!_timers.ContainsKey(e.SourceId))
                    _timers[e.SourceId] = new Timer(obj => CheckTimeout(e) , null, PollingFrequency, PollingFrequency);
                else
                    throw new InvalidOperationException("Timeout has already been requested for this id");
            }
        }
    }
}