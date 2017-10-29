using Chronos.Core.Common.Events;
using Chronos.Core.Scheduling.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using RequestTimeoutCommand = Chronos.Core.Scheduling.Commands.RequestTimeoutCommand;

namespace Chronos.Core.Scheduling.Commands
{
    public class RequestTimeoutHandler : ICommandHandler<RequestTimeoutCommand>
    {
        private readonly IEventBus _eventBus;
        private readonly IScheduler _scheduler;

        public RequestTimeoutHandler(IEventBus eventBus, IScheduler scheduler)
        {
            _eventBus = eventBus;
            _scheduler = scheduler;
        }

        public void Handle(RequestTimeoutCommand command)
        {
            _scheduler.ScheduleTimeout(command.ScheduleId, command.Duration, 
                e => _eventBus.Alert(e));
            
            _eventBus.Alert(new TimeoutRequested
            {
                ScheduleId = command.ScheduleId
            });
        }
    }
}