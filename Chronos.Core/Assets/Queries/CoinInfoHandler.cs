using System;
using Chronos.Core.Assets.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;
using NodaTime;

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

            //var otherExpression = Expression.Select<Guid, CoinHistory>(CreateAction);
                
            //otherExpression.Invoke();
                
            Expression.Invoke();
                
        }
            
        private Action<CoinHistory> CreateAction(CoinInfo info)
        {
            var timestamp = FromUtc(info.TimestampUtc);
            var price = info.Price;

            Action<CoinHistory> action = x => x.Update(timestamp, price); 
            return action;
        }
    
        private Instant FromUtc(DateTime utc)
        {
            if (utc.Kind == DateTimeKind.Unspecified)
                return Instant.MinValue;
            
            return Instant.FromDateTimeUtc(utc);
        }
            
        public CoinInfo Handle(CoinInfoQuery query)
        {
            CoinInfo coinInfo;

            if (query.CoinId == Guid.Empty)
                coinInfo = _repository.Find<CoinInfo>(c => string.Equals(c.Name,query.Name,
                    StringComparison.OrdinalIgnoreCase));
            else
                coinInfo = _repository.Find<Guid,CoinInfo>(query.CoinId);
                
            //if(coinInfo == null)
            //    throw new InvalidOperationException("No coin found");

            return coinInfo;

        }
    }
}