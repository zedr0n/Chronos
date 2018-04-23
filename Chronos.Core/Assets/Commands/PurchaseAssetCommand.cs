using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class PurchaseAssetCommand : CommandBase
    {
        public Guid AssetId { get; }
        public double Quantity { get; }
        public double CostPerUnit { get; }

        public PurchaseAssetCommand(Guid assetId, double quantity, double costPerUnit)
        {
            AssetId = assetId;
            Quantity = quantity;
            CostPerUnit = costPerUnit;
        }
    }
}