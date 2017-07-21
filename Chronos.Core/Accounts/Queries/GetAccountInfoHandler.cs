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

        private readonly IProjectionHandler<AccountInfo> _projector;

        public GetAccountInfoHandler(IProjectionHandler<AccountInfo> projector, IReadRepository repository, IProjectionManager projectionManager)
        {
            _projector = projector;
            _repository = repository;
            _projectionManager = projectionManager;
        }

        public AccountInfo Handle(GetAccountInfo query)
        {
            var projection = _projectionManager.Create<AccountInfo>()
                .From(new List<string> { StreamExtensions.StreamName<Account>(query.AccountId)})
                .When(_projector)
                .OutputState(query.AccountId);

            projection.Start();

            var accountInfo = _repository.Find<Guid, AccountInfo>(query.AccountId);

            if (query.AsOf != Instant.MaxValue)
            {
                var historicalProjection = projection.AsOf(query.AsOf);
                projection.Start();
                accountInfo = historicalProjection.State;
            }

            return accountInfo;
        }
    }
}