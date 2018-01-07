using System.Collections.Generic;

namespace Chronos.Core.Net.Parsing.Json
{
    public class CoinInfo
    {
        public string id { get; set; }
        public double price_usd { get; set; }
        public double percent_change_1h { get; set; }
        public double percent_change_24h { get; set; }
        public double percent_change_7d { get; set; }
    }
    
    public class Coin
    {
        public List<CoinInfo> Coins { get; set; }    
    }
}