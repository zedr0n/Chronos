using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class DepositCash : CommandHandlerBase,ICommandHandler<DepositCashCommand>
    {

        public DepositCash(IDomainRepository domainRepository)
            : base(domainRepository)
        {
        }

        public void Handle(DepositCashCommand command)
        {
            var account = Repository.Get<Account>(command.AggregateId);
            account.Debit(command.Amount);

            Repository.Save(account);
        }
    }
}