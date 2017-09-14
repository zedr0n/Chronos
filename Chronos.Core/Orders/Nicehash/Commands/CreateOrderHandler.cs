using System;
using Chronos.Core.Assets;
using Chronos.Core.Assets.Projections;
using Chronos.Core.Assets.Queries;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Orders.NiceHash.Commands
{
    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand>
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IQueryHandler<CoinInfoQuery, CoinInfo> _coinInfoHandler;
        
        private Guid _btcId;

        public CreateOrderHandler(IDomainRepository domainRepository, IQueryHandler<CoinInfoQuery, CoinInfo> coinInfoHandler)
        {
            _domainRepository = domainRepository;
            _coinInfoHandler = coinInfoHandler;

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
            var amount = new Amount(_btcId, command.Price);
		    var order = new Order(command.OrderId,command.OrderNumber,amount);    
           
            _domainRepository.Save(order);
        }
    }
}