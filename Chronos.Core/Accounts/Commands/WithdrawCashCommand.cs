using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class WithdrawCashCommand : CommandBase
    {
        /// <summary>
        /// Cash amount to withdraw
        /// </summary>
        public double Amount { get; set; }
    }
}