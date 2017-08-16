using System;

namespace Chronos.Core.Accounts
{
    /// <summary>
    /// Value object representing a position in an asset
    /// </summary>
    public class Position
    {
        public Guid AssetId { get; }
        public double Amount { get; }

        public Position(Guid assetId, double amount)
        {
            AssetId = assetId;
            Amount = amount;
        }

        public Position Add(double amount)
        {
            return new Position(AssetId,Amount + amount);
        }

        public Position Remove(double amount)
        {
            return new Position(AssetId, Amount - amount);
        }
    }
}