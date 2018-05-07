using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Exchanges.Events
{
    public class ExchangeOrderFilled : EventBase
    {
        public ExchangeOrderFilled(Guid exchangeId, Guid fromAsset, Guid toAsset, double fromQuantity, double toQuantity)
        {
            ExchangeId = exchangeId;
            FromAsset = fromAsset;
            ToAsset = toAsset;
            FromQuantity = fromQuantity;
            ToQuantity = toQuantity;
        }

        public Guid ExchangeId { get; }
        public Guid FromAsset { get; }
        public Guid ToAsset { get; }
        public double FromQuantity { get; }
        public double ToQuantity { get; }
    }
}