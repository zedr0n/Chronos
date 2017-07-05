using Chronos.Infrastructure.Events;

namespace Chronos.Core.Account.Events
{
    public class AccountChanged : EventBase
    {
        public string Name { get; set; }
        public string Currency { get; set; }
    }
}