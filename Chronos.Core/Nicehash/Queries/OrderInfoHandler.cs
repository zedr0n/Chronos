using System;
using Chronos.Core.Nicehash.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections.New;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Nicehash.Queries
{
    public class OrderInfoHandler : IQueryHandler<OrderInfoQuery, OrderInfo>
        {
            private readonly IReadRepository _repository;
            public IProjectionExpression<OrderInfo> Expression { get; }
    
            public OrderInfoHandler(IReadRepository repository, IProjectionManager manager)
            {
                _repository = repository;
                
                Expression = manager.Create<OrderInfo>()
                    .From<Order>()
                    .ForEachStream()
                    .OutputState();
    
                 Expression.Invoke();
            }
    
            public OrderInfo Handle(OrderInfoQuery query)
            {
                OrderInfo orderInfo;

                if (query.OrderNumber != null)
                    orderInfo = _repository.Find<OrderInfo>(o => o.OrderNumber == query.OrderNumber);
                else
                    orderInfo = _repository.Find<Guid,OrderInfo>(query.OrderId);
                //if(orderInfo == null)
                //    throw new InvalidOperationException("Order info not found");

                return orderInfo;
            }
        }    
}