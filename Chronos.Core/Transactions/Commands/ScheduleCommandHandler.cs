using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions.Commands
{
    public class ScheduleCommandHandler : ICommandHandler<ScheduleCommand>
    {
        private readonly ITimeline _timeline;
        private readonly IEventStoreConnection _connection;

        public ScheduleCommandHandler(ITimeline timeline, IEventStoreConnection connection)
        {
            _timeline = timeline;
            _connection = connection;
        }

        public void Handle(ScheduleCommand command)
        {
            _connection.Subscriptions.Alert(
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