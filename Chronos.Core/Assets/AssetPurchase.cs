using System;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Assets
{
    public class AssetPurchase : AggregateBase
    {
        public AssetPurchase() {}

        
        public AssetPurchase(Guid purchaseId, Guid assetId, double quantity, double costPerUnit)
        {
            When(new AssetPurchased(purchaseId, assetId, quantity, costPerUnit));    
        }
        
        public void When(AssetPurchased e)
        {
            Id = e.PurchaseId;
            base.When(e);   
        }
    }
}