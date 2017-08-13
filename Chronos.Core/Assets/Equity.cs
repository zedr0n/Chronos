﻿using System;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets
{
    public class Equity : AggregateBase
    {
        private string _ticker;
        private double _price;

        public Equity(Guid id, string ticker, double price)
        {
            When(new EquityCreated
            {
                EquityId = id,
                Ticker = ticker,
                Price = price
            });
        }

        private void When(EquityCreated e)
        {
            _ticker = e.Ticker;
            _price = e.Price;
            base.When(e);
        }
    }
}