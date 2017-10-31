using System.Collections.Generic;

namespace Chronos.Core.Net.Parsing.Json
{
    public class CoinInfo
    {
        public string id { get; set; }
        public double price_usd { get; set; }
    }
    
    public class Coin
    {
        public List<CoinInfo> Coins { get; set; }    
    }
}