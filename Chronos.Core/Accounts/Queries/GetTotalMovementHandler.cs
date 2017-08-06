using System;
using System.Text;
using Chronos.Core.Accounts.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections.New;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Accounts.Queries
{
    public class GetTotalMovementHandler : IQueryHandler<GetTotalMovement,TotalMovement>
    {
        private readonly IReadRepository _repository;
        private readonly Guid _requestId;

        public GetTotalMovementHandler(IReadRepository repository, IProjectionManager manager)
        {
            _repository = repository;
            _requestId = Guid.NewGuid();

            Expression = manager.Create<TotalMovement>()
                .From<Account>()
                .OutputState(_requestId);
            
            Expression.Compile();
        }

        public IProjectionExpression<TotalMovement> Expression { get; }
        public TotalMovement Handle(GetTotalMovement query)
        {
            var total = _repository.Find<Guid, TotalMovement>(_requestId);
            return total;
        }
    }
}