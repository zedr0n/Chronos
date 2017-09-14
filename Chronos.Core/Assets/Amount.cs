using System;

namespace Chronos.Core.Assets
{
    /// <summary>
    /// Value object representing a position in an asset
    /// </summary>
    public class Amount
    {
        public Guid AssetId { get; }
        public double Quantity { get; }

        public Amount(Guid assetId, double quantity)
        {
            AssetId = assetId;
            Quantity = quantity;
        }

        public static Amount Null() => new Amount(Guid.Empty,0);
        
        public Amount Add(double quantity)
        {
            return new Amount(AssetId,Quantity + quantity);
        }

        public Amount Remove(double quantity)
        {
            return new Amount(AssetId, Quantity - quantity);
        }
    }
}