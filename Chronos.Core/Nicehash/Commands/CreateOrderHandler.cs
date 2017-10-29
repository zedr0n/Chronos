using System;
using Chronos.Core.Assets;
using Chronos.Core.Assets.Projections;
using Chronos.Core.Assets.Queries;
using Chronos.Core.Common;
using Chronos.Core.Nicehash.Projections;
using Chronos.Core.Nicehash.Queries;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Nicehash.Commands
{
    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand>
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IQueryHandler<CoinInfoQuery, CoinInfo> _coinInfoHandler;
        private readonly IQueryHandler<OrderInfoQuery, OrderInfo> _orderInfoHandler;
        
        private Guid _btcId;

        public CreateOrderHandler(IDomainRepository domainRepository, IQueryHandler<CoinInfoQuery, CoinInfo> coinInfoHandler, IQueryHandler<OrderInfoQuery, OrderInfo> orderInfoHandler)
        {
            _domainRepository = domainRepository;
            _coinInfoHandler = coinInfoHandler;
            _orderInfoHandler = orderInfoHandler;

            _btcId = _coinInfoHandler.Handle(new CoinInfoQuery
            {
                Name = "Bitcoin"
            })?.Key ?? Guid.Empty;
        }

        public void Handle(CreateOrderCommand command)
        {
            if (_btcId == Guid.Empty)
            {
                _btcId = _coinInfoHandler.Handle(new CoinInfoQuery
                {
                    Name = "Bitcoin"
                })?.Key ?? Guid.Empty;    
            }

            if (_orderInfoHandler.Handle(new OrderInfoQuery
            {
                OrderNumber = command.OrderNumber
            }) != null)
                return;
            
            var amount = new Amount(_btcId, command.Price);
		    var order = new Order(command.TargetId,command.OrderNumber,amount);    
           
            _domainRepository.Save(order);
        }
    }
}