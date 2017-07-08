using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class DebitAmountHandler : ICommandHandler<DebitAmountCommand>
    {
        private readonly IDomainRepository _domainRepository;

        public DebitAmountHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(DebitAmountCommand command)
        {
            var account = _domainRepository.Get<Account>(command.AggregateId);
            account.Debit(command.Amount);

            _domainRepository.Save(account);
        }
    }
}