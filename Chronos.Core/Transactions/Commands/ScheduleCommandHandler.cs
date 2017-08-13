using System;
using System.Collections.Generic;
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
            var events = new List<CommandScheduled>
            {
                new CommandScheduled
                {
                    ScheduleId = command.ScheduleId,
                    Command = command.Command,
                    Time = command.Date,
                    Timestamp = _timeline.Now()
                }
            };
            _connection.Writer.AppendToNull(events);
        }
    }
}