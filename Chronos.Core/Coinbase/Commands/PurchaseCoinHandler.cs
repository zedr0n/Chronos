using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Coinbase.Commands
{
    public class PurchaseCoinHandler : ICommandHandler<PurchaseCoinCommand>
    {
    	private readonly IDomainRepository _domainRepository;
    
    	public PurchaseCoinHandler(IDomainRepository domainRepository)
    	{
   			_domainRepository = domainRepository;
    	}
    
    	public void Handle(PurchaseCoinCommand command)
	    {
		    var account = _domainRepository.Get<CoinbaseAccount>(command.TargetId);
		    account.PurchaseCoin(command.PurchaseId,command.Coin, command.Quantity, command.CostPerUnit, command.Fee);
    		_domainRepository.Save(account);
    	}
    }
}