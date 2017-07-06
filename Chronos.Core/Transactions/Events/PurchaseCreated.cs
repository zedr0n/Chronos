using System;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Core.Transactions.Events
{
    public class PurchaseCreated : EventBase
    {
        public Guid AccountId { get; set; }

        public string Payee { get; set; }
        public double Amount { get; set; }
        public string Ccy { get; set; }
        public Instant Date { get; set; }
    }
}