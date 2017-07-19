using System;
using System.Linq;
using Chronos.Core.Accounts.Projections;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;
using NodaTime;

namespace Chronos.Core.Accounts.Queries
{
    public class GetAccountInfoHandler : IQueryHandler<GetAccountInfo, AccountInfo>
    {
        private readonly IProjectionRepository _projections;

        private readonly IProjector<Guid,AccountInfo> _projector;

        public GetAccountInfoHandler(IProjectionRepository projections, AccountInfoProjector projector)
        {
            _projections = projections;
            _projector = projector;
        }

        public AccountInfo Handle(GetAccountInfo query)
        {
            _projector.Assign<Account>(query.AccountId).AsOf(query.AsOf).Start();
            var projection = _projections.Find<Guid, AccountInfo>(new HistoricalKey<Guid>(query.AccountId, query.AsOf));

            return projection;
        }
    }
}