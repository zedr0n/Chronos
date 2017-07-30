using System;

namespace Chronos.Infrastructure.Events
{
    public class TransactionEvent : EventBase
    {
        public Guid TransactionId { get; set; }
    }
}