using System.Collections.Generic;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public class RequestTimeoutHandler : CommandHandlerBase, ICommandHandler<RequestTimeoutCommand>
    {
        private readonly IEventStoreConnection _connection;
        private readonly IEventBus _eventBus;
        public RequestTimeoutHandler(IDomainRepository domainRepository, IEventStoreConnection connection, IEventBus eventBus) : base(domainRepository)
        {
            _connection = connection;
            _eventBus = eventBus;
        }

        public void Handle(RequestTimeoutCommand command)
        {
            var events = new List<IEvent>
            {
                new TimeoutRequested
                {
                    SourceId = command.AggregateId,
                    When = command.When
                }
            };

            _connection.AppendToNull(events);

            _eventBus.Publish(events[0]);
        }
    }
}