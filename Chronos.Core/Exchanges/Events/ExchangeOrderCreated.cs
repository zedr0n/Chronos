using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Exchanges.Events
{
    public class ExchangeOrderCreated : EventBase
    {
        public ExchangeOrderCreated(Guid exchangeId, Guid assetFromId, Guid assetToId, double fromQuantity, double toQuantity)
        {
            ExchangeId = exchangeId;
            AssetFromId = assetFromId;
            AssetToId = assetToId;
            FromQuantity = fromQuantity;
            ToQuantity = toQuantity;
        }

        public Guid ExchangeId { get; }
        public Guid AssetFromId { get; }
        public Guid AssetToId { get; }
        public double FromQuantity { get; }
        public double ToQuantity { get; }
    }
}