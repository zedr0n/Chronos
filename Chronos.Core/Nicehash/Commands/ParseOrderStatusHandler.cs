using System;
using System.Linq;
using Chronos.Core.Net.Json.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Nicehash.Commands
{
    public class ParseOrderStatusHandler : ICommandHandler<ParseOrderStatusCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly IJsonConnector _jsonConnector;
        
        public ParseOrderStatusHandler(IEventStore eventStore, IJsonConnector jsonConnector)
        {
            _eventStore = eventStore;
            _jsonConnector = jsonConnector;
        }

        public void Handle(ParseOrderStatusCommand command)
        {
            var result = _jsonConnector.Find<Json.Orders>(command.RequestId);
            if(result == null)
                throw new InvalidOperationException("Request not completed yet");

            var orderStatus = result.Result.Orders.SingleOrDefault(x => x.Id == command.OrderNumber);
            
            var @event = new OrderStatusParsed
            {
                RequestId = command.RequestId,
                Speed = orderStatus.Accepted_Speed,
                Spent = orderStatus.Btc_paid
            };
            
            _eventStore.Alert(@event);
        }
    }


}