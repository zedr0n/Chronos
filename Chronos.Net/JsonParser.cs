using Chronos.Core.Net.Parsing;
using Newtonsoft.Json;

namespace Chronos.Net
{
    public class JsonParser : IJsonParser
    {
        public T Parse<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}