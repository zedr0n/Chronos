using System;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions.Commands
{
    public class ScheduleCommandHandler : ICommandHandler<ScheduleCommand>
    {
        private readonly IEventBus _eventBus;

        public ScheduleCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Handle(ScheduleCommand command)
        {
            var guid = Guid.NewGuid();
            _eventBus.Publish(new CommandScheduled
            {
                SourceId = guid,
                ScheduleId = guid,
                Command = command.Command,
                Time = command.Date
            });
        }
    }
}