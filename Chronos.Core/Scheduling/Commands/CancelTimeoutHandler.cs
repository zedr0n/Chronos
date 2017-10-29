using Chronos.Core.Common.Commands;
using Chronos.Core.Common.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Scheduling.Commands
{
    public class CancelTimeoutHandler : ICommandHandler<CancelTimeoutCommand>
    {
        private readonly IScheduler _scheduler;
        private readonly IEventBus _eventBus;

        public CancelTimeoutHandler(IScheduler scheduler, IEventBus eventBus)
        {
            _scheduler = scheduler;
            _eventBus = eventBus;
        }

        public void Handle(CancelTimeoutCommand command)
        {
            _scheduler.Cancel(command.ScheduleId);
            _eventBus.Alert(new TimeoutCanceled
            {
                ScheduleId = command.ScheduleId
            });
        }
    }
}