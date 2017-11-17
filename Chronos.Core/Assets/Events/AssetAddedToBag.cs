using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets.Events
{
    public class AssetAddedToBag : EventBase
    {
        public AssetAddedToBag(Guid bagId, Guid assetId, double quantity)
        {
            AssetId = assetId;
            Quantity = quantity;
            BagId = bagId;
        }

        public Guid BagId { get; }
        public Guid AssetId { get; }
        public double Quantity { get; }
    }
}