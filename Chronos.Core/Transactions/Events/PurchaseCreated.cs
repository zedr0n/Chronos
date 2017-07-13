using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions.Events
{
    public class PurchaseCreated : EventBase
    {
        public Guid AccountId { get; set; }

        public string Payee { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
    }
}