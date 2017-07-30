using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions.Events
{
    public abstract class TransferCreated : TransactionEvent
    {
        public Guid FromAccount { get; set; }
        public Guid ToAccount { get; set; }

        public string Description { get; set; }
    }
}