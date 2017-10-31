using Chronos.Core.Net.Tracking.Commands;

namespace Chronos.Core.Net.Tracking.Urls
{
    public class OrderUrlProvider : IUrlProvider
    {
        private readonly string format = "https://api.nicehash.com/api?method=orders.get&my&id={0}&key={1}&location=0&algo={2}";
        private readonly int nicehashAlgo = 29;
        private readonly string _key;
        private readonly int _id;

        public OrderUrlProvider(string key, int id)
        {
            _key = key;
            _id = id;
        }

        public string Get(params object[] args)
        {
            var url = string.Format(format, _id , _key, nicehashAlgo);
            return url;
        }
    }
}