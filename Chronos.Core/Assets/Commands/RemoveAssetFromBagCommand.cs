using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class RemoveAssetFromBagCommand : CommandBase 
    {
        public RemoveAssetFromBagCommand(Guid assetId, double quantity)
        {
            AssetId = assetId;
            Quantity = quantity;
        }

        /// <summary>
        /// Asset id
        /// </summary>
        public Guid AssetId { get; }
        /// <summary>
        /// Asset quantity
        /// </summary>
        public double Quantity { get; }
    }
}