using System.Collections.Generic;

namespace Chronos.Core.Net.Parsing.Json
{
    public class OrderStatusJson
    {
        public int Id { get; set; }
        public double Accepted_Speed { get; set; }
        public double Btc_paid { get; set; }
    }

    public class Result
    {
        public List<OrderStatusJson> Orders { get; set; }
    }
    
    public class Orders
    {
        public Result Result { get; set; }
    }
}