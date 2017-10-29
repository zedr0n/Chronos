using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using Chronos.Core.Common.Commands;
using Chronos.Core.Common.Events;
using Chronos.Core.Scheduling.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Core.Scheduling.Commands
{
    public class RequestStopAtHandler : ICommandHandler<RequestStopAtCommand>
    {
        private readonly IEventBus _eventBus;
        private readonly IScheduler _scheduler;
        
        public RequestStopAtHandler(IScheduler scheduler, IEventBus eventBus)
        {
            _scheduler = scheduler;
            _eventBus = eventBus;
        }
        
        public void Handle(RequestStopAtCommand command)
        {
            _scheduler.ScheduleStop(command.ScheduleId, command.When,
                e => _eventBus.Alert(new StopCompleted
                {
                    ScheduleId = e.ScheduleId,
                    Timestamp = e.Timestamp
                }));
            
            _eventBus.Alert(new StopRequested
            {
                ScheduleId = command.ScheduleId
            }); 
        }
    }
}