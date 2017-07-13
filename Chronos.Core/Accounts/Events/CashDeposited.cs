using Chronos.Infrastructure.Events;

namespace Chronos.Core.Accounts.Events
{
    public class CashDeposited : EventBase
    {
        public double Amount { get; set; }
    }
}