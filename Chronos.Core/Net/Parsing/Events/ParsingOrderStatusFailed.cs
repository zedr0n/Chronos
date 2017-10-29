using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Parsing.Events
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