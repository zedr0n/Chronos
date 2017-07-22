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
        private readonly IProjectionManager _projectionManager;


        public GetAccountInfoHandler(IReadRepository repository, IProjectionManager projectionManager)
        {
            _repository = repository;
            _projectionManager = projectionManager;
        }

        public AccountInfo Handle(GetAccountInfo query)
        {
            var projection = _projectionManager.Create<AccountInfo>()
                .From<Guid,Account>(query.AccountId)
                .OutputState(query.AccountId);

            projection.Start();

            var accountInfo = _repository.Find<Guid, AccountInfo>(query.AccountId);

            if (query.AsOf != Instant.MaxValue)
            {
                var historicalProjection = _projectionManager.Create<AccountInfo>()
                    .From<Guid,Account>(query.AccountId)
                    .AsOf(query.AsOf);

                projection.Start();
                accountInfo = historicalProjection.State;
            }

            return accountInfo;
        }
    }
}