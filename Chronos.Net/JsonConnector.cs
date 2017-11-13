using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Newtonsoft.Json;

namespace Chronos.Net
{
    public class JsonConnector : IJsonConnector
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
        
        private readonly Subject<string> _urls = new Subject<string>();
        private IObservable<Envelope> Requests { get; } 
        
        public void SubmitRequest(string url)
        {
            _urls.OnNext(url);
        }

        public JsonConnector()
        {
            Requests = _urls.AsObservable().Select(s =>
                    new Envelope(s,new Lazy<IObservable<string>>(
                        () => Observable.FromAsync(() => GetAsync(s)))))
                .DelayBetweenValues(TimeSpan.FromSeconds(2));
        }

        public IObservable<Lazy<IObservable<string>>> GetRequest(string url)
        {
            return Requests.Where(x => x.Url == url).Select(x => x.Observable);
        }
        
        private async Task<string> GetAsync(string url) 
        {
            using (var w = new HttpClient())
            {
                var json = await w.GetStringAsync(url);

                if (string.IsNullOrEmpty(json))
                    return null;

                return json;
            }
        }
    }
}