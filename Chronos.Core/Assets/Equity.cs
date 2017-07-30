using System;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets
{
    public class Equity : Asset, IConsumer<EquityCreated>
    {
        private string _ticker;
        private double _price;

        public Equity(Guid id, string ticker, double price)
            : base(id)
        {
            RaiseEvent(new EquityCreated
            {
                EquityId = id,
                Ticker = ticker,
                Price = price
            });
        }

        public void When(EquityCreated e)
        {
            _ticker = e.Ticker;
            _price = e.Price;
        }
    }
}