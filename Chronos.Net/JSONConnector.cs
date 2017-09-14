using System;
using System.Net.Http;
using Chronos.Infrastructure;
using Newtonsoft.Json;

namespace Chronos.Net
{
    public class JSONConnector : IJSONConnector
    {
        public T Get<T>(string url)
            where T : new()
        {
            using (var w = new HttpClient())
            {
                var jsonData = string.Empty;
                try
                {
                    jsonData = w.GetStringAsync(url).Result;
                }
                catch (Exception) { }

                return !string.IsNullOrEmpty(jsonData) ? JsonConvert.DeserializeObject<T>(jsonData) : new T();
            }
        }
    }
}