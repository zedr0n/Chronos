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
            if(_domainRepository.Exists<Account>(command.AggregateId))
                throw new InvalidOperationException("Account has already been created");
                
            var account = new Account(command.AggregateId,command.Name, command.Currency);
            _domainRepository.Save(account);
        }
    }
}