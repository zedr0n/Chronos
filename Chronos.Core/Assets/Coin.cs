using System;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Assets
{
    public class Coin : AggregateBase
    {
        private string _ticker;
        private string _name;
        
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
    }
}