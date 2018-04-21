using System;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Assets
{
    /// <summary>
    /// Crypto coin aggregate root
    /// </summary>
    public class Coin : AggregateBase, IAsset
    {
        private string _ticker;
        private string _name;
        private double _price;
        
        public Coin() {}
        public Coin(Guid coinId, string ticker,string name)
        {
            When(new CoinCreated
            {
                CoinId = coinId,
                Name = name,
                Ticker = ticker
            });
        }

        public void When(CoinCreated e)
        {
            Id = e.CoinId;
            _ticker = e.Ticker;
            _name = e.Name;
            base.When(e);
        }
        
        private void When(AssetPriceUpdated e)
        {
            _price = e.Price;
            base.When(e);
        }

        private void When(AssetChangeUpdated e)
        {
            base.When(e);
        }

        public void UpdateChange(double hourChange, double dayChange, double weekChange)
        {
            When(new AssetChangeUpdated(Id,hourChange,dayChange,weekChange));
        }
        
        public void UpdatePrice(double price)
        {
             When(new AssetPriceUpdated
             {
                 AssetId = Id,
                 Price = price 
             });
        }
    }
}