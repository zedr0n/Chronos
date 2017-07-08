using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class CreateAccountHandler : ICommandHandler<CreateAccountCommand>
    {
        private readonly IDomainRepository _domainRepository;
        public CreateAccountHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(CreateAccountCommand command)
        {
            var account = _domainRepository.Find<Account>(command.AggregateId);
            if(account != null)
                throw new InvalidOperationException("Account has already been created");
                
            account = new Account(command.AggregateId,command.Name, command.Currency);
            _domainRepository.Save(account);
        }
    }
}