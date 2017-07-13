using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets.Events
{
    public class EquityCreated : EventBase
    {
        public string Ticker { get; set; }
        public double Price { get; set; }
    }
}