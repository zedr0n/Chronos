using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Core.Assets.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;
using NodaTime;

namespace Chronos.Core.Assets.Queries
{
    public class BagInfoHandler : IQueryHandler<BagInfoQuery, BagInfo>
    {
        private readonly IReadRepository _repository;
        public IProjectionExpression<BagInfo> Expression { get; }
        private readonly IDebugLog _debugLog;
            
        public BagInfoHandler(IReadRepository repository, IProjectionManager manager, IDebugLog debugLog)
        {
            _repository = repository;
            _debugLog = debugLog;
            Expression = manager.Create<BagInfo>()
                .From<Bag>()
                .Include<Coin>()
                .ForEachStream()
                //.Do(Log)
                .OutputState();

            //var otherExpression = Expression.Select<Guid, BagHistory>(CreateAction);
            
            //otherExpression.Invoke();
            Expression.Invoke();

        }

        private void Log(BagInfo bagInfo)
        {
            if (bagInfo.Name == "Btc")
            {
                _debugLog.WriteLine(bagInfo.Name + "(" + bagInfo.TimestampUtc + ") : " + bagInfo.Value );
            } 
        }

        private Action<BagHistory> CreateAction(BagInfo bagInfo)
        {
            var timestamp = FromUtc(bagInfo.TimestampUtc);
            //Log(bagInfo);

            var value = bagInfo.Value;
            var name = bagInfo.Name;

            Action<BagHistory> action = bagHistory =>
            {
                bagHistory.Name = name;
                if (bagHistory.Name == "Btc" && value != 0)
                {
                    var found = true;
                }
                bagHistory.Update(timestamp, value);
            };
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
            
            //foreach(var coinInfo in bagInfo.Assets.Select(x => _repository.Find<Guid,CoinInfo>(x)).Where(x => x != null))
            //    bagInfo.UpdatePrice(coinInfo.Key,coinInfo.Price);
            
            return bagInfo;
        }
    }
}