using System;
using Chronos.Core.Assets.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections.New;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Assets.Queries
{
    public class BagInfoHandler : IQueryHandler<BagInfoQuery, BagInfo>
    {
        private readonly IReadRepository _repository;
        public IProjectionExpression<BagInfo> Expression { get; }
    
        public BagInfoHandler(IReadRepository repository, IProjectionManager manager)
        {
            _repository = repository;
            Expression = manager.Create<BagInfo>()
                .From<Bag>()
                .Include<Coin>()
                .ForEachStream()
                .OutputState();
            Expression.Invoke();
        }
    
        public BagInfo Handle(BagInfoQuery query)
        {
            return _repository.Find<Guid,BagInfo>(query.BagId);
        }
    }
}