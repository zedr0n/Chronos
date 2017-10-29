using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Common.Events
{
    public class ParsingOrderStatusFailed : EventBase
    {
        public ParsingOrderStatusFailed(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
    }
}