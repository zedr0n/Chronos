using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets.Events
{
    public class EquityCreated : EventBase
    {
        public Guid EquityId { get; set; }
        public string Ticker { get; set; }
        public double Price { get; set; }
    }
}