using System;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Common.Events
{
    public class CoinInfoParsed : AssetJsonParsed
    {
        public CoinInfoParsed(Guid coinId, double priceUsd)
            : base(coinId)
        {
            PriceUsd = priceUsd;
        }

        public double PriceUsd { get; }
    }
}