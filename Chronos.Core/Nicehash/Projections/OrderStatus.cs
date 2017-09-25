using System;
using Chronos.Core.Nicehash.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Nicehash.Projections
{
    public class OrderStatus : ReadModelBase<Guid>
    {
        public Guid OrderId => Key;
        public int OrderNumber { get; set; }
        public double Spent { get; set; }
        public double Speed { get; set; }

        private void When(NicehashOrderCreated e)
        {
            Spent = 0.0;
            Speed = 0.0;
            OrderNumber = e.OrderNumber;
        }
        
        private void When(NicehashOrderUpdated e)
        {
            Spent = e.Spent;
            Speed = e.Speed;
        }
    }
}