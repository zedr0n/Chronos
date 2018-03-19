using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class DepositCashCommand : CommandBase
    {
        /// <summary>
        /// Cash amount to deposit
        /// </summary>
        public double Amount { get; set; }
    }
}