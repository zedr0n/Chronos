using System;
using Chronos.Core.Nicehash.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Nicehash.Projections
{
    public class OrderInfo : ReadModelBase<Guid>
    {
        public int OrderNumber { get; set; }
        public double MaxSpeed { get; set; }
        public Guid PriceAsset { get; set; }

        private void When(NicehashOrderCreated e)
        {
            OrderNumber = e.OrderNumber;
            MaxSpeed = e.MaxSpeed;
            PriceAsset = e.PriceAsset;
        }
    }
}