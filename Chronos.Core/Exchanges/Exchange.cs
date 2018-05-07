using System;
using System.Collections.Generic;
using Chronos.Core.Exchanges.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Core.Exchanges
{
    public class Exchange : AggregateBase
    {
        private string _name;

        private readonly Dictionary<Guid, double> _openOrders = new Dictionary<Guid, double>();
        
        public Exchange() {}
        
        public Exchange(Guid exchangeId, string name)
        {
            When(new ExchangeAdded(exchangeId, name));
        }

        public void CreateOrder(Guid assetFrom, Guid assetTo, double quantityFrom, double quantityTo)
        {
            When(new ExchangeOrderCreated
                (Id,assetFrom, assetTo, quantityFrom, quantityTo));
        }

        public void FillOrder(Guid assetFrom, Guid assetTo, double quantityFrom, double quantityTo)
        {
            if (_openOrders.TryGetValue(assetFrom, out var orders) && orders >= quantityFrom)
            {
                When(new ExchangeOrderFilled
                    (Id, assetFrom, assetTo,quantityFrom, quantityTo));     
            }
            else
                throw new InvalidOperationException("No oustanding order to fill");
        }

        public void When(ExchangeOrderCreated e)
        {
            _openOrders.TryGetValue(e.AssetFromId, out var orders);
            _openOrders[e.AssetFromId] = orders + e.FromQuantity;
        }

        public void When(ExchangeOrderFilled e)
        {
            _openOrders[e.FromAsset] -= e.FromQuantity;
        }

        public void When(ExchangeAdded e)
        {
            Id = e.ExchangeId;
            _name = e.Name;
        }

        protected override void When(IEvent e)
        {
            When((dynamic) e);
        }
    }
}