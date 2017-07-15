using System;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions
{
    public class AssetTransfer : Transfer, IConsumer<AssetTransferCreated>
    {
        private Guid _assetId;

        public AssetTransfer() { }
        public AssetTransfer(Guid id) : base(id) { }

        public AssetTransfer(Guid id, Guid accountFrom, Guid accountTo, Guid assetId, string description)
        {
            RaiseEvent(new AssetTransferCreated
            {
                SourceId = id,
                FromAccount = accountFrom,
                ToAccount = accountTo,
                AssetId = assetId,
                Description = description
            });
        }

        public void When(AssetTransferCreated e)
        {
            if (e.SourceId != Id)
                return;

            TransferDetails = new TransferDetails(e.FromAccount,e.ToAccount);

            _assetId = e.AssetId;
            Description = e.Description;
        }
    }
}