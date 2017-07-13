using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class DepositCashCommand : CommandBase
    {
        public double Amount { get; set; }
    }
}