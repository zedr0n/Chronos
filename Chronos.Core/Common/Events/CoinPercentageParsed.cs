using System;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Common.Events
{
    public class CoinPercentageParsed : AssetJsonParsed 
    {
        public double HourChange { get; set; }
        public double DayChange { get; set; }
        public double WeekChange { get; set; }
        public override bool Finish() => false;
        
        public CoinPercentageParsed(Guid id,
            double percentage1h, double percentage24h, double percentage7d)
         : base(id)
        {
            HourChange = percentage1h;
            DayChange = percentage24h;
            WeekChange = percentage7d;
        }
    }
}