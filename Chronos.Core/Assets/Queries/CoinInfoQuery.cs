using System;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Assets.Queries
{
    public class CoinInfoQuery : IQuery<CoinInfoQuery>
    {
        public string Name { get; set; }
        public Guid CoinId { get; set; }
    }
}