using System;
using Chronos.Core.Assets.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections.New;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Assets.Queries
{
    public class CoinInfoHandler : IQueryHandler<CoinInfoQuery, CoinInfo>
        {
            private readonly IReadRepository _repository;
            public IProjectionExpression<CoinInfo> Expression { get; }
    
            public CoinInfoHandler(IReadRepository repository, IProjectionManager manager)
            {
                _repository = repository;

                Expression = manager.Create<CoinInfo>()
                    .From<Coin>()
                    .ForEachStream()
                    .OutputState();
                
                 Expression.Invoke();
            }
    
            public CoinInfo Handle(CoinInfoQuery query)
            {
                CoinInfo coinInfo;

                if (query.CoinId == Guid.Empty)
                    coinInfo = _repository.Find<CoinInfo>(c => c.Name == query.Name);
                else
                    coinInfo = _repository.Find<Guid,CoinInfo>(query.CoinId);
                
                if(coinInfo == null)
                    throw new InvalidOperationException("No coin found");

                return coinInfo;

            }
        }
}