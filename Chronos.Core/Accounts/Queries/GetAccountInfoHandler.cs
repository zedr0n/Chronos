using System;
using System.Linq;
using Chronos.Core.Accounts.Projections;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Accounts.Queries
{
    public class GetAccountInfoHandler : IQueryHandler<GetAccountInfo, AccountInfo>
    {
        private readonly IProjectionRepository _projections;
        private readonly IProjectionManager _manager;

        public GetAccountInfoHandler(IProjectionRepository projections, IProjectionManager manager)
        {
            _projections = projections;
            _manager = manager;
        }

        public AccountInfo Handle(GetAccountInfo query)
        {
            _manager.RegisterJuncture<AccountInfo>(p => p.AccountId == query.AccountId, query.AsOf);
            var projection = _projections.Find<AccountInfo>(p => p.AccountId == query.AccountId && p.AsOf.CompareTo(query.AsOf) == 0 )?.SingleOrDefault();

            if (projection == null)
                throw new InvalidOperationException("Read model has not been found");

            return projection;
        }
    }
}