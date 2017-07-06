using System;
using Chronos.Core.Accounts.Projections;
using Chronos.Infrastructure.Queries;
using NodaTime;

namespace Chronos.Core.Accounts.Queries
{
    public class GetAccountInfo : IQuery<AccountInfo>
    {
        public Guid AccountId { get; set; }
        public Instant AsOf { get; set; } = Instant.MaxValue;
    }
}