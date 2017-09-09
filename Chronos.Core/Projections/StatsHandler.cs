using Chronos.Core.Accounts;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections.New;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Projections
{
    public class StatsHandler : IQueryHandler<StatsQuery,Stats>
    {
        private readonly IReadRepository _repository;

        public StatsHandler(IReadRepository repository, IProjectionManager manager)
        {
            _repository = repository;

            Expression = manager.Create<Stats>().From<Account>()
                .OutputState("Global");     
            Expression.Invoke();
        }

        public IProjectionExpression<Stats> Expression { get; }
        public Stats Handle(StatsQuery query)
        {
            var stats = _repository.Find<string, Stats>(query.Id);
            return stats;
        }
    }
}