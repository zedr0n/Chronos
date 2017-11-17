using System;
using Chronos.Core.Common;

namespace Chronos.Core.Assets
{
    public struct Position
    {
        public Position(Guid assetId, Amount amount)
        {
            AssetId = assetId;
            Amount = amount;
        }

        public Amount Amount { get; }
        public Guid AssetId { get; }
    }
}