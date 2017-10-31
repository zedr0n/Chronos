using System;

namespace Chronos.Core.Net.Tracking.Urls
{
    public class CoinUrlProvider : IUrlProvider
    {
        private static readonly string Format = "https://api.coinmarketcap.com/v1/ticker/{0}";
        private static readonly string ConvertFormat = Format + "/?convert={1}";
        
        public string Get(params object[] args)
        {
            switch (args.Length)
            {
                default:
                    throw new ArgumentNullException();
                case 1:
                    return string.Format(Format, args[0]);
                case 2:
                    return string.Format(ConvertFormat, args[0], args[1]);
            }
        }
    }
}