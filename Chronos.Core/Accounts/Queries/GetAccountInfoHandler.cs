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

            var projection = _projectionManager.Create<AccountInfo>().From<Account>().ForEachStream().OutputState();
            projection.Start();
        }

        public AccountInfo Handle(GetAccountInfo query)
        {
            var accountInfo = _repository.Find<Guid, AccountInfo>(query.AccountId);

            if (query.AsOf != Instant.MaxValue)
            {
                var historicalProjection = _projectionManager.Create<AccountInfo>()
                    .From<Account>(query.AccountId)
                    .AsOf(query.AsOf);

                historicalProjection.Start();
                accountInfo = historicalProjection.State;
            }

            return accountInfo;
        }
    }
}