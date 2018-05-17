using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class CreateAssetExchangeHandler : ICommandHandler<CreateAssetExchangeCommand>
    {
    		private readonly IDomainRepository _domainRepository;
    
    		public CreateAssetExchangeHandler(IDomainRepository domainRepository)
    		{
    				_domainRepository = domainRepository;
    		}
    
    		public void Handle(CreateAssetExchangeCommand command)
    		{
    			var exchange = new AssetExchange(command.AssetFrom, command.AssetTo, command.QuantityFrom, command.QuantityTo, command.ExchangeId);        
    			_domainRepository.Save(exchange);
    		}
    }
    
    
}