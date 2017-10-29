using System;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Common.Events
{
    public class OrderStatusParsed : EventBase 
    {
        public OrderStatusParsed(Guid orderId,double speed, double spent) 
        {
            Speed = speed;
            Spent = spent;
            OrderId = orderId;
        }

        public Guid OrderId { get; }
        public double Speed { get; }
        public double Spent { get; }
    }
}