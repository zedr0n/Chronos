using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class DepositCashHandler : CommandHandlerBase,ICommandHandler<DepositCashCommand>
    {
        public DepositCashHandler(IDomainRepository domainRepository)
            : base(domainRepository)
        {
        }

        /// <summary>
        /// <see cref="Account.Debit"/>
        ///     (<see cref="DepositCashCommand.Amount"/>)
        ///  ->  @ <see cref="Account"/> : <see cref="Chronos.Core.Accounts.Events.CashDeposited"/>
        /// </summary>
        public void Handle(DepositCashCommand command)
        {
            var account = Repository.Get<Account>(command.TargetId);
            account.Debit(command.Amount);

            Repository.Save(account);
        }
    }
}