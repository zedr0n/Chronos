using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Exchanges.Commands
{
    public class FillExchangeOrderHandler : ICommandHandler<FillExchangeOrderCommand>
    {
        private readonly IDomainRepository _domainRepository;

        public FillExchangeOrderHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(FillExchangeOrderCommand command)
        {
            var exchange = _domainRepository.Find<Exchange>(command.TargetId);
            exchange?.FillOrder(command.FromAssetId, command.ToAssetId, command.FromQuantity, command.ToQuantity);
            _domainRepository.Save(exchange);
        }
    }
}