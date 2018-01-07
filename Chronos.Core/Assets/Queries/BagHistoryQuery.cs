using System;
using Chronos.Core.Assets.Projections;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Assets.Queries
{
    public class BagHistoryQuery : IQuery<BagHistory>
    {
        public Guid BagId { get; }
        
        public BagHistoryQuery() {}

        public BagHistoryQuery(Guid badId)
        {
            BagId = badId;
        }
    }
}