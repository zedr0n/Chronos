using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Chronos.Infrastructure;

namespace Chronos.Net
{
    public class FakeJsonConnector : IJsonConnector
    {
        private class Envelope
        {
            public Envelope(string url, Lazy<IObservable<string>> observable)
            {
                Url = url;
                Observable = observable;
            }

            public string Url { get; }
            public Lazy<IObservable<string>> Observable { get; }
        }
        
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

        private string Provide(string url)
        {
            return _json.TryGetValue(url, out var json) ? json : "";
        }

        public FakeJsonConnector()
        {
            Requests = _urls.AsObservable().Select(s =>
                    new Envelope(s, new Lazy<IObservable<string>>(
                        () => Observable.Return(Provide(s)))))
                .DelayBetweenValues(TimeSpan.FromSeconds(0.05));
        }

        private readonly Subject<string> _urls = new Subject<string>(); 
        private IObservable<Envelope> Requests { get; } 
        public void SubmitRequest(string url)
        {
            _urls.OnNext(url);
        }

        public IObservable<Lazy<IObservable<string>>> GetRequest(string url)
        {
            return Requests.Where(x => x.Url == url).Select(x => x.Observable);
            //return _urls.Select(s => new Lazy<IObservable<string>>(() => Observable.Return(_json[url])));
            //return Observable.Return();
        }
    }
}