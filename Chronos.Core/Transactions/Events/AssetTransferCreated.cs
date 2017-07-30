using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions.Events
{
    public class AssetTransferCreated : EventBase
    {
        public Guid TransferId { get; set; }
        public Guid AssetId { get; set; }

        public Guid FromAccount { get; set; }
        public Guid ToAccount { get; set; }

        public string Description { get; set; }
    }
}