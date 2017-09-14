using System;
using System.Collections.Generic;
using System.Net.Http;
using Chronos.Infrastructure;
using Newtonsoft.Json;

namespace Chronos.Net
{
    public class JsonConnector : IJsonConnector
    {
        private readonly Dictionary<Guid,object> _repository = new Dictionary<Guid, object>();
        
        public T Get<T>(string url) 
            where T : class
        {
            using (var w = new HttpClient())
            {
                var jsonData = string.Empty;
                try
                {
                    jsonData = w.GetStringAsync(url).Result;
                }
                catch (Exception) { }

                if (string.IsNullOrEmpty(jsonData))
                    return null;
                
                var jObject = JsonConvert.DeserializeObject<T>(jsonData);
                return jObject;
            }
        }

        public void Save<T>(Guid requestId, T result) where T : class
        {
            _repository[requestId] = result;
        }

        public T Find<T>(Guid requestId)
            where T : class
        {
            if (_repository.ContainsKey(requestId))
                return _repository[requestId] as T;
            return null;
        }
    }
}