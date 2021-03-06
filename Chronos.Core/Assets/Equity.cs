﻿using System;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Assets
{
    public class Equity : AggregateBase, IAsset
    {
        private string _ticker;
        private double _price;

        public Equity() {}
        
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

        private void When(AssetPriceUpdated e)
        {
            _price = e.Price;
            base.When(e);
        }

        public void UpdatePrice(double price)
        {
             When(new AssetPriceUpdated
             {
                 AssetId = Id,
                 Price = price 
             });
        }

        /// <summary>
        /// Update asset price change
        /// </summary>
        /// <param name="hourChange">Percentage change in an hour</param>
        /// <param name="dayChange">Percentage change in 24 hours</param>
        /// <param name="weekChange">Percentage change in 7 days</param>
        public void UpdateChange(double hourChange, double dayChange, double weekChange)
        {
        }
    }
}