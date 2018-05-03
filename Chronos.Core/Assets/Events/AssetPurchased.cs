using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets.Events
{
    public class AssetPurchased : EventBase
    {
        public Guid PurchaseId { get; }
        public Guid AssetId { get; }
        public double Quantity { get; }
        public double CostPerUnit { get; }

        public AssetPurchased(Guid purchaseId, Guid assetId, double quantity, double costPerUnit)
        {
            PurchaseId = purchaseId;
            AssetId = assetId;
            Quantity = quantity;
            CostPerUnit = costPerUnit;
        }
    }
}