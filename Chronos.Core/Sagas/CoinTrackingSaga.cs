using System;
using Chronos.Core.Assets;
using Chronos.Core.Assets.Commands;
using Chronos.Core.Common.Events;
using Chronos.Core.Net.Parsing.Commands;
using Chronos.Core.Net.Parsing.Json;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class CoinTrackingSaga : AssetTrackingSaga,
        //IHandle<CoinTrackingRequested>,
        IHandle<CoinInfoParsed>,
        IHandle<CoinPercentageParsed>
    {
        private Guid _coinId;
        private string _ticker;
        private double _price;
        private double _hourChange;
        private double _dayChange;
        private double _weekChange;

        public CoinTrackingSaga()
        {
            Register<CoinTrackingRequested>(Trigger.TrackingRequested); 
            Register<CoinInfoParsed>(Trigger.Parsed);
        }
        
        protected override void Handle(IEvent e) => When((dynamic) e); 
        
        protected void When(CoinTrackingRequested e)
        {
            _coinId = e.AssetId;
            _ticker = e.Ticker; 
            base.When(e);
        }

        protected override void OnReceived(string json)
        {
            if (json == null)
                return;
            
            var command = new ParseCoinCommand(_coinId, _ticker, json);
            SendMessage(command);
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