using Chronos.Infrastructure.Events;

namespace Chronos.Core.Accounts.Events
{
    public class CashWithdrawn : EventBase
    {
        public double Amount { get; set; }
    }
}