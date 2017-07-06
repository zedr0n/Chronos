using Chronos.Infrastructure.Events;

namespace Chronos.Core.Accounts.Events
{
    public class AccountCreated : EventBase
    {
        public string Name { get; set; }
        public string Currency { get; set; }
    }
}