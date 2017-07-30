using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions.Events
{
    public class CashTransferCreated : EventBase
    {
        public Guid TransferId { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }

        public Guid FromAccount { get; set; }
        public Guid ToAccount { get; set; }

        public string Description { get; set; }

    }
}