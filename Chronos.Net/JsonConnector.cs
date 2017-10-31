using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Newtonsoft.Json;

namespace Chronos.Net
{
    public class JsonConnector : IJsonConnector
    {
        public IObservable<string> Request(string url)
        {
            return Observable.FromAsync(() => GetAsync(url)); //.Timeout(DateTimeOffset.Now.AddSeconds(10));
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