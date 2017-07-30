using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Accounts.Events
{
    public class CashDeposited : EventBase
    {
        public Guid AccountId { get; set; }
        public double Amount { get; set; }
    }
}