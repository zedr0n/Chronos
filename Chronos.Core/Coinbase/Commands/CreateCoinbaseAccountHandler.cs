using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Coinbase.Commands
{
    public class CreateCoinbaseAccountHandler : ICommandHandler<CreateCoinbaseAccountCommand>
    {
    	private readonly IDomainRepository _domainRepository;
    
    	public CreateCoinbaseAccountHandler(IDomainRepository domainRepository)
    	{
    		_domainRepository = domainRepository;
    	}
    
    	public void Handle(CreateCoinbaseAccountCommand command)
    	{
    		var account = new Account(command.TargetId,command.Email);        
    		_domainRepository.Save(account);
    	}
    }
}