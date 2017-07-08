using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class DebitAmountCommand : CommandBase
    {
        public double Amount { get; set; }
    }
}