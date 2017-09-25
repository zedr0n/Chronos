using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Nicehash.Events
{
    public class NicehashOrderCreated  : EventBase
    {
        public Guid OrderId { get; set; }
        public int OrderNumber { get; set; }
        public Guid PriceAsset { get; set; } 
        public double Price { get; set; }
        public double MaxSpeed { get; set; }
    }
}