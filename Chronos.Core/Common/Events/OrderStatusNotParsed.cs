using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Common.Events
{
    public class OrderStatusNotParsed : EventBase
    {
        public OrderStatusNotParsed(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
        
    }
}