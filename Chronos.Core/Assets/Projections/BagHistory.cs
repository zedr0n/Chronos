using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure;
using NodaTime;
using NodaTime.Text;

namespace Chronos.Core.Assets.Projections
{
    [Reset,MemoryProxy]
    public class BagHistory : ReadModelBase<Guid>
    {
        private readonly SortedDictionary<long, double> _values = new SortedDictionary<long, double>();
        public string Values { get; set; }
        
        public void Update(Instant time, double value)
        {
            var previousValue = _values
                .LastOrDefault(x => x.Key < time.ToUnixTimeTicks())
                .Value;
            
            if(value != previousValue)
                _values[time.ToUnixTimeTicks()] = value;
            //Serialize();
        }

        private void Serialize()
        {
            Values = "";
            foreach (var val in _values)
                Values += InstantPattern.ExtendedIso.Format(Instant.FromUnixTimeTicks(val.Key)) + " : " + val.Value + " ; ";
        }
    }
}