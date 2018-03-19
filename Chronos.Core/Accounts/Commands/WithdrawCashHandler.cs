using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class WithdrawCashHandler : CommandHandlerBase, ICommandHandler<WithdrawCashCommand>
    {
        public WithdrawCashHandler(IDomainRepository domainRepository) : base(domainRepository)
        {
        }

        /// <summary>
        /// <see cref="Account.Credit"/>
        ///     (<see cref="WithdrawCashCommand.Amount"/>)
        ///  -> @ <see cref="Account"/> : <see cref="Chronos.Core.Accounts.Events.CashWithdrawn"/> 
        /// </summary>
        /// <param name="command">Command to withdraw cash</param>
        public void Handle(WithdrawCashCommand command)
        {
            var account = Repository.Get<Account>(command.TargetId);
            account.Credit(command.Amount);
            Repository.Save(account);
        }
    }
}