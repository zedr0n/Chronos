using System;
using System.Collections.Generic;
using Chronos.Core.Accounts.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections.New;
using Chronos.Infrastructure.Queries;
using NodaTime;

namespace Chronos.Core.Accounts.Queries
{
    public class GetAccountInfoHandler : IQueryHandler<GetAccountInfo, AccountInfo>
    {
        private readonly IReadRepository _repository;
        public IProjection<AccountInfo> Projection { get; }

        public GetAccountInfoHandler(IReadRepository repository, IProjectionManager projectionManager)
        {
            _repository = repository;

            var projection = projectionManager.Create<AccountInfo>()
                .From<Account>()
                .ForEachStream()
                .OutputState();

            projection.Start();
            Projection = (IProjection<AccountInfo>) projection;
        }


        public AccountInfo Handle(GetAccountInfo query)
        {
            var accountInfo = _repository.Find<Guid, AccountInfo>(query.AccountId);
            return accountInfo;
        }
    }
}