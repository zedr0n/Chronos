using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Orders.NiceHash.Events
{
    public class NicehashOrderUpdated : EventBase
    {
        public Guid OrderId { get; set; }
        public double Spent { get; set; }
        public double Speed { get; set; }
    }
}