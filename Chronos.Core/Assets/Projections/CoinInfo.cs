using System;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Assets.Projections
{
    //[Reset]
    /// <summary>
    /// Coin info read model
    /// </summary>
    public class CoinInfo : ReadModelBase<Guid>
    {
        public string Name { get; set; }
        public string Ticker { get; set; }
        public double Price { get; set; }
        
        public double HourChange { get; set; }
        public double DayChange { get; set; }
        public double WeekChange { get; set; }

        private void When(CoinCreated e)
        {
            Name = e.Name;
            Ticker = e.Ticker;
        }

        private void When(AssetPriceUpdated e)
        {
            Price = e.Price;
        }

        private void When(AssetChangeUpdated e)
        {
            HourChange = e.HourChange;
            DayChange = e.DayChange;
            WeekChange = e.WeekChange;
        }
    }
}