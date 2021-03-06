﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Core.Assets.Events;
using Chronos.Core.Common;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Core.Assets
{
    /// <summary>
    /// Bag of assets
    /// </summary>
    public class Bag : AggregateBase
    {
        private readonly List<Amount> _assets = new List<Amount>();
        private readonly string _name;

        public Bag() {}
        
        public Bag(Guid bagId,string name)
        {
            When(new BagCreated(bagId,name));
        }

        protected override void When(IEvent e)
        {
            When((dynamic) e);
        }

        private void When(BagCreated e)
        {
            Id = e.BagId;
            base.When(e);
        }
        
        /// <summary>
        /// Add asset to bag
        /// </summary>
        /// <param name="assetId">Asset id</param>
        /// <param name="quantity">Asset quantity</param>
        public void Add(Guid assetId, double quantity)
        {
            When( new AssetAddedToBag(Id,assetId,quantity) );
        }

        /// <summary>
        /// Remove asset from bag
        /// </summary>
        /// <param name="assetId">Asset id</param>
        /// <param name="quantity">Asset quantity</param>
        public void Remove(Guid assetId, double quantity)
        {
            var currentQuantity = _assets.Where(x => x.EntityId == assetId).Select(x => x.Quantity).Sum();
            if (currentQuantity < quantity)
                quantity = currentQuantity;
                
            When(new AssetRemovedFromBag(assetId,quantity));
        }

        private void When(AssetAddedToBag e)
        {
            _assets.Add(new Amount(e.AssetId, e.Quantity));
            base.When(e);
        }

        private void When(AssetRemovedFromBag e)
        {
            var assetId = e.AssetId;
            var quantity = e.Quantity;

            foreach (var amount in _assets.Where(x => x.EntityId == assetId).Reverse())
            {
                if (amount.Quantity < quantity)
                {
                    amount.Substract(amount.Quantity);
                    quantity -= amount.Quantity;
                }
                else
                {
                    amount.Substract(quantity);
                    break;
                }
            }

            _assets.RemoveAll(x => x.Quantity == 0);
            base.When(e);
        }
    }
}