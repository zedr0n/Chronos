using System;
using Chronos.Core.Assets.Commands;
using Chronos.Core.Common.Events;
using Chronos.Core.Net.Parsing.Commands;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class CoinTrackingSaga : AssetTrackingSaga,
        IHandle<CoinTrackingRequested>,
        IHandle<CoinInfoParsed>
    {
        private Guid _coinId;
        private string _ticker;
        private double _price;

        public void When(CoinTrackingRequested e) => base.When(e);

        protected override void OnTracking(AssetTrackingRequested e)
        {
            _coinId = e.AssetId;
            _ticker = ((CoinTrackingRequested) e).Ticker;
            base.OnTracking(e);    
        }

        protected override void OnReceived(string json)
        {
            var command = new ParseCoinCommand(_coinId, _ticker, json);
            SendMessage(command);
            base.OnReceived(json);
        }

        public void When(CoinInfoParsed e)
        {
            _price = e.PriceUsd;
            base.When(e);    
        }

        protected override void OnParsed()
        {
            var command = new UpdateAssetPriceCommand
            {
                TargetId = _coinId,
                Price = _price
            };
            SendMessage(command);
            base.OnParsed();
        }
    }
}