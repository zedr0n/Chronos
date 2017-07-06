using System.Linq;
using Chronos.Core.Accounts.Projections;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Accounts.Queries
{
    public class GetAccountInfoHandler : IQueryHandler<GetAccountInfo, AccountInfo>
    {
        private readonly IRepository<AccountInfo> _projections;
        private readonly AccountInfoProjector _accountInfoProjector;

        public GetAccountInfoHandler(IRepository<AccountInfo> projections, AccountInfoProjector accountInfoProjector)
        {
            _projections = projections;
            _accountInfoProjector = accountInfoProjector;
        }

        public AccountInfo Handle(GetAccountInfo query)
        {
            var projection = _projections.Find(query.AccountId, p => p.AsOf.CompareTo(query.AsOf) == 0 )?.SingleOrDefault();

            if (projection == null)
                _accountInfoProjector.Rebuild(query.AsOf);

            return _projections.Get(query.AccountId, p => p.AsOf.CompareTo(query.AsOf) == 0).Single();
        }
    }
}