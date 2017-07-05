using Chronos.Infrastructure.Events;

namespace Chronos.Core.Account
{
    public class AmountDebited : EventBase
    {
        public double Amount { get; set; }
    }
}