using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using System.Reactive.Linq;
using NodaTime;

namespace Chronos.Infrastructure.Commands
{
    public class RequestTimeoutHandler : CommandHandlerBase, ICommandHandler<RequestTimeoutCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly ITimeline _timeline;

        private readonly Dictionary<Guid, IDisposable> _subscriptions = new Dictionary<Guid, IDisposable>();

        public RequestTimeoutHandler(IDomainRepository domainRepository, IEventStore eventStore, ITimeline timeline) : base(domainRepository)
        {
            _eventStore = eventStore;
            _timeline = timeline;
        }

        public void Handle(RequestTimeoutCommand command)
        {
            if(_subscriptions.ContainsKey(command.TargetId))
                _subscriptions[command.TargetId].Dispose();
            
            var when = command.When;
            
            if (when == default(Instant))
            {
                if(!_timeline.Live)
                    throw new InvalidOperationException("Timers cannot work in historical mode?");
                when = _timeline.Now().Plus(command.Duration);
            }
            lock (_subscriptions)
                _subscriptions[command.TargetId] = _timeline.StopAt(when, new TimeoutCompleted
                {
                    ScheduleId = command.TargetId
                })
                .Subscribe(e =>
                {
                    //lock(_subscriptions)
                        _subscriptions[e.ScheduleId].Dispose();
                    //_connection.Writer.AppendToNull(new[] {e});
                    _eventStore.Alert(e);
                });
        }
    }
}