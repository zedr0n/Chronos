using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Accounts.Events
{
    public class AccountChanged : EventBase
    {
        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
    }
}