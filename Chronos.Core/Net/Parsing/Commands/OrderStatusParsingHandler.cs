using System.Linq;
using Chronos.Core.Common.Commands;
using Chronos.Core.Common.Events;
using Chronos.Core.Net.Parsing.Events;
using Chronos.Core.Nicehash.Json;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Parsing.Commands
{
    public class OrderStatusParsingHandler : ICommandHandler<ParseOrderCommand> 
    {
        private readonly IEventBus _eventBus;
        private readonly IJsonParser _parser;

        public OrderStatusParsingHandler(IEventBus eventBus, IJsonParser parser) 
        {
            _eventBus = eventBus;
            _parser = parser;
        }

        public void Handle(ParseOrderCommand command)
        {
            var parsed = _parser.Parse<Orders>(command.Json);
            var orders = parsed?.Result?.Orders;
            var status = orders?.SingleOrDefault(x => x.Id == command.OrderNumber );

            if (status == null)
                _eventBus.Alert(
                    new ParsingOrderStatusFailed(command.AssetId));
            else
                _eventBus.Alert(
                new OrderStatusParsed(command.AssetId,status.Accepted_Speed, status.Btc_paid)); 
        }
    }
}