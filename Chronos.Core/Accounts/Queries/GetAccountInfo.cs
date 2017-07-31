using System;
using Chronos.Core.Accounts.Projections;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Accounts.Queries
{
    public class GetAccountInfo : IQuery<AccountInfo>
    {
        public Guid AccountId { get; set; }
    }
}