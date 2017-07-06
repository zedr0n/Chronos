using Chronos.Infrastructure.Events;

namespace Chronos.Core.Accounts.Events
{
    public class AmountDebited : EventBase
    {
        public double Amount { get; set; }
    }
}