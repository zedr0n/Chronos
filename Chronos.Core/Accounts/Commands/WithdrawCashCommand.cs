using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class WithdrawCashCommand : CommandBase
    {
        public double Amount { get; set; }
    }
}