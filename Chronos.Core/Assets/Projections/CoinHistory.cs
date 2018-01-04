using System;
using System.Collections.Generic;
using Chronos.Infrastructure;
using NodaTime;

namespace Chronos.Core.Assets.Projections
{
    [Reset,MemoryProxy]
    public class CoinHistory : ReadModelBase<Guid>
    {
        private readonly Dictionary<long, double> _values = new Dictionary<long, double>();
        public string Values { get; set; }
        
        public void Update(Instant time, double value)
        {
            _values[time.ToUnixTimeTicks()] = value;
            //Serialize();
        }
    }
}