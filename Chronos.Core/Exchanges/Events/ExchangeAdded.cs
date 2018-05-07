using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Exchanges.Events
{
    public class ExchangeAdded : EventBase
    {
        public ExchangeAdded(Guid exchangeId, string name)
        {
            ExchangeId = exchangeId;
            Name = name;
        }

        public Guid ExchangeId { get; }
        public string Name { get; }
    }
}