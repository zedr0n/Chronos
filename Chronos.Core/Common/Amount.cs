using System;

namespace Chronos.Core.Common
{
    /// <summary>
    /// Value object representing a position in an asset
    /// </summary>
    public class Amount
    {
        public Guid EntityId { get; }
        public double Quantity { get; private set; }

        public Amount(Guid entityId, double quantity)
        {
            EntityId = entityId;
            Quantity = quantity;
        }

        public static Amount Null() => new Amount(Guid.Empty,0);
        
        public void Add(double quantity)
        {
            Quantity += quantity;
        }

        public void Substract(double quantity)
        {
            Quantity -= quantity;
        }
    }
}