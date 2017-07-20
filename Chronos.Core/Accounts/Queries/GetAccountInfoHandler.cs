using System;
using System.Collections.Generic;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Projections.New;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Queries;
using NodaTime;

namespace Chronos.Core.Accounts.Queries
{
    public class GetAccountInfoHandler : IQueryHandler<GetAccountInfo, AccountInfo>
    {
        private readonly IReadRepository _repository;

        private readonly Infrastructure.Projections.New.IProjection<Guid,AccountInfo> _projection;

        public GetAccountInfoHandler(Infrastructure.Projections.New.IProjection<Guid, AccountInfo> projection, IReadRepository repository)
        {
            _projection = projection;
            _repository = repository;
        }

        public AccountInfo Handle(GetAccountInfo query)
        {
            _projection.From(new List<string> { StreamExtensions.StreamName<Account>(query.AccountId) })
                .ForAllStreams(query.AccountId).OutputState().Start();

            var accountInfo = _repository.Find<Guid, AccountInfo>(query.AccountId);

            if (query.AsOf != Instant.MaxValue)
            {
                var projection = _projection.From(new List<string> { StreamExtensions.StreamName<Account>(query.AccountId) })
                    .ForAllStreams(query.AccountId).AsOf(query.AsOf);
                projection.Start();
                accountInfo = projection.State;
            }

            return accountInfo;
        }
    }
}