using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Common.Events
{
    public class OrderStatusParsed : AssetJsonParsed 
    {
        public OrderStatusParsed(Guid orderId,double speed, double spent) 
            : base(orderId)
        {
            Speed = speed;
            Spent = spent;
        }

        public double Speed { get; }
        public double Spent { get; }
    }
}