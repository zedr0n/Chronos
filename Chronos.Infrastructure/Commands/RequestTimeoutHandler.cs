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
        private readonly IEventStoreConnection _connection;
        private readonly ITimeline _timeline;

        private IDisposable _subscription;
        
        public RequestTimeoutHandler(IDomainRepository domainRepository, IEventStoreConnection connection, ITimeline timeline) : base(domainRepository)
        {
            _connection = connection;
            _timeline = timeline;
        }

        public void Handle(RequestTimeoutCommand command)
        {
            /*_connection.Writer.AppendToNull(new[] { new TimeoutRequested
            {
                ScheduleId = command.TargetId,
                When = command.When
            } });*/

            var when = command.When;
            
            if (when == default(Instant))
            {
                if(!_timeline.Live)
                    throw new InvalidOperationException("Timers cannot work in historical mode?");
                when = _timeline.Now().Plus(command.Duration);
            }
            
            _subscription = _timeline.StopAt(when, new TimeoutCompleted
                {
                    ScheduleId = command.TargetId
                })
                .Subscribe(e =>
                {
                    lock(_subscription)
                        _subscription.Dispose();
                    //_connection.Writer.AppendToNull(new[] {e});
                    _connection.Subscriptions.SendTransient(e);
                });
        }
    }
}