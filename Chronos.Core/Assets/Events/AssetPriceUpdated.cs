using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets.Events
{
    public class AssetPriceUpdated : EventBase
    {
        public Guid AssetId { get; set; }
        public double Price { get; set; }

        public AssetPriceUpdated()
        {
            Insertable = true;
        }
    }
}