using System;
using System.Linq;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using NodaTime;
using NodaTime.Extensions;

namespace Chronos.Infrastructure
{
    public class DefaultReplayStrategy : IReplayStrategy
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IEventStore _eventStore;
        private readonly ICommandBus _commandBus;

        public DefaultReplayStrategy(IDomainRepository domainRepository, IEventStore eventStore, ICommandBus commandBus)
        {
            _domainRepository = domainRepository;
            _eventStore = eventStore;
            _commandBus = commandBus;
        }

        public void Replay(Instant date)
        {
            _eventStore.Alert(new ReplayRequested
            {
                Timestamp = date
            });
            
            _domainRepository.Reset();
            
            var commands = _eventStore.Connection.ReadCommands(DateTime.MinValue.ToUniversalTime().ToInstant(), date, _eventStore.Timeline.TimelineId)
                .OfType<RequestTimeoutCommand>();
            
            foreach(var command in commands)
                _commandBus.Send(command);
            
            _eventStore.Alert(new ReplayCompleted
            {
                Timestamp = date
            });
        }
    }
}