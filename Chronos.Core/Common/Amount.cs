using System;

namespace Chronos.Core.Common
{
    /// <summary>
    /// Value object representing a position in an asset
    /// </summary>
    public class Amount
    {
        public Guid EntityId { get; }
        public double Quantity { get; }

        public Amount(Guid entityId, double quantity)
        {
            EntityId = entityId;
            Quantity = quantity;
        }

        public static Amount Null() => new Amount(Guid.Empty,0);
        
        public Amount Add(double quantity)
        {
            return new Amount(EntityId,Quantity + quantity);
        }

        public Amount Substract(double quantity)
        {
            return new Amount(EntityId, Quantity - quantity);
        }
    }
}