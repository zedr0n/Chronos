using System;

namespace Chronos.Core.Transactions.Events
{
    public class AssetTransferCreated : TransferCreated
    {
        public Guid AssetId { get; set; }
    }
}