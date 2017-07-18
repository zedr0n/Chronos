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

        private readonly AccountInfoProjector _projector;

        public GetAccountInfoHandler(IProjectionRepository projections, AccountInfoProjector projector)
        {
            _projections = projections;
            _projector = projector;
        }

        public AccountInfo Handle(GetAccountInfo query)
        {
            if (!query.AsOf.Equals(Instant.MaxValue))
            {
                var projection = _projections.Find<HistoricalKey<Guid>, HistoricalProjection<Guid,AccountInfo>>(new HistoricalKey<Guid>
                {
                    AsOf = query.AsOf,
                    Key = query.AccountId
                });

                if (projection == null)
                {
                    var projector = _projector.WithAccount(query.AccountId)
                        .AsOf<AccountInfoProjector, Guid, AccountInfo>(query.AsOf);
                    projector.Start();

                    projection = _projections.Find<HistoricalKey<Guid>, HistoricalProjection<Guid, AccountInfo>>(new HistoricalKey<Guid>
                    {
                        AsOf = query.AsOf,
                        Key = query.AccountId
                    });
                }

                return projection.Projection;
            }
            else
            {
                var projection = _projections.Find<Guid, AccountInfo>(query.AccountId);

                if (projection == null)
                {
                    _projector.WithAccount(query.AccountId).Start();
                    projection = _projections.Find<Guid, AccountInfo>(query.AccountId);
                }

                return projection;
            }
        }
    }
}