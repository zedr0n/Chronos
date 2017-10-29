using Chronos.Core.Common.Commands;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Scheduling.Commands
{
    public class ScheduleCommandHandler : ICommandHandler<ScheduleCommand>
    {
        private readonly ITimeline _timeline;
        private readonly IEventBus _eventBus;

        public ScheduleCommandHandler(ITimeline timeline, IEventBus eventBus)
        {
            _timeline = timeline;
            _eventBus = eventBus;
        }

        public void Handle(ScheduleCommand command)
        {
            _eventBus.Alert(
                new CommandSchedulingRequested
                {
                    ScheduleId = command.ScheduleId,
                    Command = command.Command,
                    When = command.Date,
                    Timestamp = _timeline.Now()
                });
        }
    }
}