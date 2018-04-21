using System;
using Chronos.Core.Assets;
using Chronos.Core.Assets.Commands;
using Chronos.Core.Common.Events;
using Chronos.Core.Net.Parsing.Commands;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class CoinTrackingSaga : AssetTrackingSaga,
        IHandle<CoinTrackingRequested>,
        IHandle<CoinInfoParsed>,
        IHandle<CoinPercentageParsed>
    {
        private Guid _coinId;
        private string _ticker;
        private double _price;
        private double _hourChange;
        private double _dayChange;
        private double _weekChange;

        public void When(CoinTrackingRequested e) => base.When(e);

        protected override void When(IEvent e)
        {
            When((dynamic) e);
        }

        protected override void OnTracking(AssetTrackingRequested e)
        {
            _coinId = e.AssetId;
            _ticker = ((CoinTrackingRequested) e).Ticker;
            base.OnTracking(e);    
        }

        protected override void OnReceived(string json)
        {
            if (json != null)
            {
                var command = new ParseCoinCommand(_coinId, _ticker, json);
                SendMessage(command);    
            }
            base.OnReceived(json);
        }

        public void When(CoinPercentageParsed e)
        {
            _dayChange = e.DayChange;
            _weekChange = e.WeekChange;
            _hourChange = e.HourChange;
            base.When(e);
        }
        
        public void When(CoinInfoParsed e)
        {
            _price = e.PriceUsd;
            base.When(e);    
        }

        protected override void OnParsed()
        {
            var command = new UpdateCoinPriceCommand 
            {
                TargetId = _coinId,
                Price = _price
            };
            SendMessage(command);
            
            base.OnParsed();
        }
    }
}