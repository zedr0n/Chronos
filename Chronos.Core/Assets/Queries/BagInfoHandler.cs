using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Core.Assets.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;
using NodaTime;

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

            var otherExpression = Expression.Select<Guid, BagHistory>(CreateAction);
            
            otherExpression.Invoke();
            Expression.Invoke();

        }

        private Action<BagHistory> CreateAction(BagInfo bagInfo)
        {
            var timestamp = FromUtc(bagInfo.TimestampUtc);
            var value = bagInfo.Value;

            Action<BagHistory> action = bagHistory => bagHistory.Update(timestamp, value);
            return action;
        }

        private Instant FromUtc(DateTime utc)
        {
            if (utc.Kind == DateTimeKind.Unspecified)
                return Instant.MinValue;
            
            return Instant.FromDateTimeUtc(utc);
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