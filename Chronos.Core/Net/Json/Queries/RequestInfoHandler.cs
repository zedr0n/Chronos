using System;
using Chronos.Core.Net.Json.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections.New;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Net.Json.Queries
{
    public class RequestInfoHandler<T> : IQueryHandler<RequestInfoQuery<T>,RequestInfo<T>> where T : class
    {
        private readonly IReadRepository _readRepository;

        public RequestInfoHandler(IProjectionManager projectionManager, IReadRepository readRepository)
        {
            _readRepository = readRepository;
            
            Expression = projectionManager.Create<RequestInfo<T>>()
                .From<Request<T>>()
                .ForEachStream()
                .OutputState();
            
            Expression.Invoke();
        }

        public IProjectionExpression<RequestInfo<T>> Expression { get; }

        public RequestInfo<T> Handle(RequestInfoQuery<T> query)
        {
            var requestInfo = _readRepository.Find<Guid,RequestInfo<T>>(query.RequestId);
            return requestInfo;
        }
    }
}