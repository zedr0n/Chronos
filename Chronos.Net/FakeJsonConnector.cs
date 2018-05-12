using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Chronos.Infrastructure;

namespace Chronos.Net
{
    public class FakeJsonConnector : IJsonConnector
    {
        private readonly Dictionary<string,string> _json = new Dictionary<string, string>()
        {
            {"https://api.coinmarketcap.com/v1/ticker/Bitcoin", @"[
                {
                    ""id"": ""bitcoin"", 
                    ""name"": ""Bitcoin"", 
                    ""symbol"": ""BTC"", 
                    ""rank"": ""1"", 
                    ""price_usd"": ""8422.11"", 
                    ""price_btc"": ""1.0"", 
                    ""24h_volume_usd"": ""7072610000.0"", 
                    ""market_cap_usd"": ""143426848878"", 
                    ""available_supply"": ""17029800.0"", 
                    ""total_supply"": ""17029800.0"", 
                    ""max_supply"": ""21000000.0"", 
                    ""percent_change_1h"": ""0.81"", 
                    ""percent_change_24h"": ""-2.45"", 
                    ""percent_change_7d"": ""-14.74"", 
                    ""last_updated"": ""1526148272""
                }
            ]"} 
        };

        
        public void SubmitRequest(string url)
        {
            if(!_json.ContainsKey(url))
                throw new InvalidOperationException("Url not found");
        }

        public IObservable<Lazy<IObservable<string>>> GetRequest(string url)
        {
            return Observable.Return(new Lazy<IObservable<string>>(() => Observable.Return(_json[url])));
        }
    }
}