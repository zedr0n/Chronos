using System.Collections.Generic;

namespace Chronos.Core.Nicehash.Json
{
    public class OrderStatus
    {
        public int Id { get; set; }
        public double Accepted_Speed { get; set; }
        public double Btc_paid { get; set; }
    }

    public class Result
    {
        public List<OrderStatus> Orders { get; set; }
    }
    
    public class Orders
    {
        public Result Result { get; set; }
    }
}