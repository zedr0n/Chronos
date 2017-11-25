using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Core.Assets.Projections
{
    [Reset]
    public class BagInfo : ReadModelBase<Guid>
    {
        private readonly Dictionary<Guid, double> _assets = new Dictionary<Guid, double>();
        private readonly Dictionary<Guid, double> _prices = new Dictionary<Guid, double>();

        public double Value { get; set; }
        public int NumberOfAssets => _assets.Count;
        
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

        private void When(StateReset e)
        {
            _assets.Clear();
            _prices.Clear();
        }

        public override void When(IEvent e)
        {
            base.When(e);
            Value = _assets.Sum(asset => _prices[asset.Key] * asset.Value); 
        }
    }
}