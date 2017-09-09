using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets.Events
{
    public class CoinCreated : EventBase
    {
        public Guid CoinId { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }
    }
}