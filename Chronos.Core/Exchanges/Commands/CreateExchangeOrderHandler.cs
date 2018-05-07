using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Exchanges.Commands
{
    public class CreateExchangeOrderHandler : ICommandHandler<CreateExchangeOrderCommand>
    {
    		private readonly IDomainRepository _domainRepository;
    
    		public CreateExchangeOrderHandler(IDomainRepository domainRepository)
    		{
    				_domainRepository = domainRepository;
    		}
    
    		public void Handle(CreateExchangeOrderCommand command)
		    {
			    var exchange = _domainRepository.Find<Exchange>(command.TargetId);
			    exchange?.CreateOrder(command.AssetFrom, command.AssetTo, command.QuantityFrom, command.QuantityTo);
    			_domainRepository.Save(exchange);
    		}
    }
    
    
}