using System;
using System.Collections.Generic;
using Chronos.Core.Net.Parsing;
using Chronos.Infrastructure;
using Newtonsoft.Json;

namespace Chronos.Net
{
    public class JsonParser : IJsonParser
    {
        public T Parse<T>(string json)
        {
            var obj = JsonConvert.DeserializeObject<T>(json);
            //if (obj == null)
            //    throw new InvalidOperationException("Cannot parse json to type " + typeof(T).SerializableName());
            return obj;
        }
    }
}