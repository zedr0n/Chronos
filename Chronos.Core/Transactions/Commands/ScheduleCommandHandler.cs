using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions.Commands
{
    public class ScheduleCommandHandler : ICommandHandler<ScheduleCommand>
    {
        private readonly ITimeline _timeline;
        private readonly IEventStore _eventStore;

        public ScheduleCommandHandler(ITimeline timeline, IEventStore eventStore)
        {
            _timeline = timeline;
            _eventStore = eventStore;
        }

        public void Handle(ScheduleCommand command)
        {
            _eventStore.Alert(
                new CommandScheduled
                {
                    ScheduleId = command.ScheduleId,
                    Command = command.Command,
                    Time = command.Date,
                    Timestamp = _timeline.Now()
                });
        }
    }
}