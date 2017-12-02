using System;
using Chronos.Core.Assets.Projections;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Assets.Queries
{
    public class BagInfoQuery : IQuery<BagInfo>
    {
        public BagInfoQuery() { }

        public BagInfoQuery(Guid bagId)
        {
            BagId = bagId;
        }
        
        public Guid BagId { get; set; }    
        public string Name { get; set; }
    }
}