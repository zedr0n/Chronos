using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Core.Assets.Events;
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
        private readonly IStateWriter _stateWriter;
        public IProjectionExpression<BagInfo> Expression { get; }
    
        public BagInfoHandler(IReadRepository repository, IProjectionManager manager, IStateWriter stateWriter)
        {
            _repository = repository;
            _stateWriter = stateWriter;
            Expression = manager.Create<BagInfo>()
                .From<Bag>()
                .Include<Coin>()
                .ForEachStream()
                //.Do(Save)
                .OutputState();
            Expression.Invoke();
        }

        private Instant FromUtc(DateTime utc)
        {
            if (utc.Kind == DateTimeKind.Unspecified)
                return Instant.MinValue;
            else
                return Instant.FromDateTimeUtc(utc);
        }

        private void Save(IEnumerable<BagInfo> bagInfos, IEnumerable<IEvent> e)
        {
            var infos = bagInfos.GroupBy(x => x.Key);
            foreach (var info in infos)
            {
                _stateWriter.Write<Guid,BagHistory>(info.Key,x =>
                {
                    foreach(var bagInfo in info.AsEnumerable())
                        x.Update(FromUtc(bagInfo.TimestampUtc),bagInfo.Value);
                    return true;
                });
            }
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