using System.Collections.Generic;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Assets.Projections
{
    [Reset,MemoryProxy]
    public class BagList
    {
        public string Name { get; set; }
        private List<string> _assets { get; set; } = new List<string>();

        private void When(BagCreated e)
        {
            
        }
        
    }
}