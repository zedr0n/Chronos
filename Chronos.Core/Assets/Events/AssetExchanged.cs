using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets.Events
{
    public class AssetExchanged : EventBase
    {
        public AssetExchanged(Guid fromAsset, Guid toAsset, double fromQuantity, double toQuantity)
        {
            FromAsset = fromAsset;
            ToAsset = toAsset;
            FromQuantity = fromQuantity;
            ToQuantity = toQuantity;
        }

        public Guid FromAsset { get; }
        public Guid ToAsset { get; }
        public double FromQuantity { get; }
        public double ToQuantity { get; }
    }
}