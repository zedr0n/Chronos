using Chronos.Infrastructure.Events;

namespace Chronos.Core.Account.Events
{
    public class AmountDebited : EventBase
    {
        public double Amount { get; set; }
    }
}