﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Core.Assets.Projections
{
    [Reset,MemoryProxy]
    public class BagInfo : ReadModelBase<Guid>
    {
        public string Name { get; set; }
        
        private readonly Dictionary<Guid, double> _assets = new Dictionary<Guid, double>();
        private readonly Dictionary<Guid, double> _prices = new Dictionary<Guid, double>();

        public double Value { get; set; }
        public int NumberOfAssets => _assets.Count;
        public IEnumerable<Guid> Assets => new List<Guid>(_assets.Keys);

        public double Quantity(Guid assetId)
        {
            return _assets.ContainsKey(assetId) ? _assets[assetId] : 0.0;
        }

        private void When(BagCreated e)
        {
            Name = e.Name;
        }
        
        private void When(AssetAddedToBag e)
        {
            if(!_assets.ContainsKey(e.AssetId))
                _assets.Add(e.AssetId,0.0);

            _assets[e.AssetId] += e.Quantity;
            _prices[e.AssetId] = 0.0;
        }

        private void When(AssetRemovedFromBag e)
        {
            _assets[e.AssetId] -= e.Quantity;
        }

        private void When(AssetPriceUpdated e)
        {
            _prices[e.AssetId] = e.Price;
        }

        public void UpdatePrice(Guid assetId, double price)
        {
            _prices[assetId] = price;
            Update();
        }

        private void When(StateReset e)
        {
            _assets.Clear();
            _prices.Clear();
        }

        public override void When(IEvent e)
        {
            base.When(e);
            Update();
        }

        private void Update()
        {
            Value = _assets.Sum(asset => _prices[asset.Key] * asset.Value);  
        }
    }
}