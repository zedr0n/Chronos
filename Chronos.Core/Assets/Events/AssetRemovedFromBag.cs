using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets.Events
{
    public class AssetRemovedFromBag : EventBase
    {
        public AssetRemovedFromBag(Guid assetId, double quantity)
        {
            AssetId = assetId;
            Quantity = quantity;
        }

        public Guid AssetId { get; }
        public double Quantity { get; }
    }
}