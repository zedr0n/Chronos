using System;
using Chronos.Core.Assets.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Assets.Queries
{
    public class BagHistoryHandler : IQueryHandler<BagHistoryQuery,BagHistory>
    {
        private readonly IReadRepository _readRepository;

        public BagHistoryHandler(IReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public IProjectionExpression<BagHistory> Expression { get; }
        public BagHistory Handle(BagHistoryQuery query)
        {
            var bagHistory = _readRepository.Find<Guid, BagHistory>(query.BagId);
            return bagHistory;
        }
    }
}