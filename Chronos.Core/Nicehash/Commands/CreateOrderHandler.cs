using Chronos.Core.Common;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Nicehash.Commands
{
    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand>
    {
        private readonly IDomainRepository _domainRepository;

        public CreateOrderHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(CreateOrderCommand command)
        {
            var amount = new Amount(command.PriceAssetId, command.Price);
		    var order = new Order(command.TargetId,command.OrderNumber,amount);    
           
            _domainRepository.Save(order);
        }
    }
}