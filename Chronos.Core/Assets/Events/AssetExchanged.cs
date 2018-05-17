using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets.Events
{
    public class AssetExchanged : EventBase
    {
        public AssetExchanged(Guid fromAsset, Guid toAsset, double fromQuantity, double toQuantity, Guid exchangeId)
        {
            FromAsset = fromAsset;
            ToAsset = toAsset;
            FromQuantity = fromQuantity;
            ToQuantity = toQuantity;
            ExchangeId = exchangeId;
        }

        public Guid ExchangeId { get; }
        public Guid FromAsset { get; }
        public Guid ToAsset { get; }
        public double FromQuantity { get; }
        public double ToQuantity { get; }
    }
}