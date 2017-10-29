using System.Linq;
using Chronos.Core.Common.Commands;
using Chronos.Core.Common.Events;
using Chronos.Core.Net.Parsing.Events;
using Chronos.Core.Nicehash.Json;
using Chronos.Core.Nicehash.Projections;
using Chronos.Core.Nicehash.Queries;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Net.Parsing.Commands
{
    public class OrderStatusParsingHandler : ICommandHandler<ParseOrderCommand> 
    {
        private readonly IEventBus _eventBus;
        private readonly IJsonParser _parser;
        private readonly IQueryHandler<OrderInfoQuery, OrderInfo> _queryHandler;
        public OrderStatusParsingHandler(IEventBus eventBus, IQueryHandler<OrderInfoQuery, OrderInfo> queryHandler, IJsonParser parser) 
        {
            _eventBus = eventBus;
            _queryHandler = queryHandler;
            _parser = parser;
        }

        public void Handle(ParseOrderCommand command)
        {
            var orderInfo = _queryHandler.Handle(new OrderInfoQuery
            {
                OrderId = command.AssetId
            });

            if (orderInfo == null)
                return;    
             
            var parsed = _parser.Parse<Orders>(command.Json);
            var orders = parsed?.Result?.Orders;
            //if (orders == null)
            //    return;
            
            var status = orders?.SingleOrDefault(x => x.Id == orderInfo.OrderNumber );

            if (status == null)
                _eventBus.Alert(
                    new ParsingOrderStatusFailed(command.AssetId));
            else
                _eventBus.Alert(
                new OrderStatusParsed(command.AssetId,status.Accepted_Speed, status.Btc_paid)); 
        }
    }
}