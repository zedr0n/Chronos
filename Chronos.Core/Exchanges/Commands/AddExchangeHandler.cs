using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Exchanges.Commands
{
    public class AddExchangeHandler : ICommandHandler<AddExchangeCommand>
    {
    		private readonly IDomainRepository _domainRepository;
    
    		public AddExchangeHandler(IDomainRepository domainRepository)
    		{
    			_domainRepository = domainRepository;
    		}
    
    		public void Handle(AddExchangeCommand command)
    		{
			    var exchange = new Exchange(command.TargetId, command.Name);
			    _domainRepository.Save(exchange);
    		}
    }
    
    
}
