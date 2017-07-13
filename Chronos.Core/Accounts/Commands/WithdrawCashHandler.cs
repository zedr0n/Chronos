using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class WithdrawCashHandler : CommandHandlerBase, ICommandHandler<WithdrawCashCommand>
    {
        public WithdrawCashHandler(IDomainRepository domainRepository) : base(domainRepository)
        {
        }

        public void Handle(WithdrawCashCommand command)
        {
            var account = Repository.Get<Account>(command.AggregateId);
            account.Credit(command.Amount);
            Repository.Save(account);
        }
    }
}