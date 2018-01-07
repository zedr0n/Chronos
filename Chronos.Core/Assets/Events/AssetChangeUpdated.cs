using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets.Events
{
    public class AssetChangeUpdated : EventBase
    {
        public Guid CoinId { get; }
        public double HourChange { get; }
        public double DayChange { get; }
        public double WeekChange { get; }

        public AssetChangeUpdated(Guid coinId, double hourChange, double dayChange, double weekChange)
        {
            CoinId = coinId;
            HourChange = hourChange;
            DayChange = dayChange;
            WeekChange = weekChange;
        }
    }
}