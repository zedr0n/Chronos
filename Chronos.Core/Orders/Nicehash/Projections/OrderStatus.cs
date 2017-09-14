using System;
using Chronos.Core.Orders.NiceHash.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Orders.NiceHash.Projections
{
    public class OrderStatus : ReadModelBase<Guid>
    {
        public int OrderNumber { get; set; }
        public double Spent { get; set; }
        public double Speed { get; set; }

        private void When(NicehashOrderCreated e)
        {
            Spent = 0.0;
            Speed = 0.0;
        }
        
        private void When(NicehashOrderUpdated e)
        {
            Spent = e.Spent;
            Speed = e.Speed;
        }
    }
}