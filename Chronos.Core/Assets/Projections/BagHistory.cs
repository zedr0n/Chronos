using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure;
using NodaTime;
using NodaTime.Text;

namespace Chronos.Core.Assets.Projections
{
    public class ValueInfo
    {
        public int Id { get; set; }
        public DateTime TimestampUtc { get; set; }
        public double Value { get; set; }
    }
    
    [Reset,MemoryProxy]
    public class BagHistory : ReadModelBase<Guid>
    {
        public string Name { get; set; }
        private readonly SortedDictionary<Instant, double> _values = new SortedDictionary<Instant, double>();
        public List<ValueInfo> Values { get; set; }

        public BagHistory()
        {
            Values = new List<ValueInfo>();
        }

        
        public void Update(Instant time, double value)
        {
            var previousValue = _values
                .LastOrDefault(x => x.Key < time)
                .Value;

            if (Math.Abs(value - previousValue) < 1e-6) 
                return;
            
            _values[time] = value;
            Values.Add(new ValueInfo
            {
                TimestampUtc = time.ToDateTimeUtc(),
                Value = value
            });
            //Serialize();
        }
    }
}