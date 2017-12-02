using System;
using System.Linq;
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
            BagInfo bagInfo;

            if (query.BagId == Guid.Empty)
                bagInfo = _repository.Find<BagInfo>(x => 
                    string.Equals(x.Name,query.Name,StringComparison.OrdinalIgnoreCase));
            else
                bagInfo = _repository.Find<Guid,BagInfo>(query.BagId);
            
            foreach(var coinInfo in bagInfo.Assets.Select(x => _repository.Find<Guid,CoinInfo>(x)).Where(x => x != null))
                bagInfo.UpdatePrice(coinInfo.Key,coinInfo.Price);
            
            return bagInfo;
        }
    }
}