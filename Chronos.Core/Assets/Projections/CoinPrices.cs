using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Assets.Projections
{
    public class CoinPrice
    {
        public int Id { get; set; }
        public Guid CoinId { get; set; }
        public double Price { get; set; }

        public CoinPrice(Guid coinId, double price)
        {
            CoinId = coinId;
            Price = price;
        }
    }
    
    public class CoinPrices : ReadModelBase<Guid>
    {
        public List<CoinPrice> Prices { get; set; }
        private IEnumerable<Guid> Coins => Prices.Select(x => x.CoinId);

        public void When(AssetPriceUpdated e)
        {
            if (Coins.Contains(e.AssetId))
                Prices.Single(x => x.CoinId == e.AssetId).Price = e.Price;
            else
                Prices.Add(new CoinPrice(e.AssetId,e.Price));
        }
    }
}