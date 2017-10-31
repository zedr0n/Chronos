using System;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Assets.Projections
{
    public class CoinInfo : ReadModelBase<Guid>
    {
        public string Name { get; set; }
        public string Ticker { get; set; }
        public double Price { get; set; }

        private void When(CoinCreated e)
        {
            Name = e.Name;
            Ticker = e.Ticker;
        }

        private void When(AssetPriceUpdated e)
        {
            Price = e.Price;
        }
    }
}