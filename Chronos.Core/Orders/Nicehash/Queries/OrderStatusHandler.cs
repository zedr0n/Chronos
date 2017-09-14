using System;
using Chronos.Core.Orders.NiceHash.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections.New;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Orders.NiceHash.Queries
{
    public class OrderStatusHandler : IQueryHandler<OrderStatusQuery, OrderStatus>
        {
            private readonly IReadRepository _repository;
            public IProjectionExpression<OrderStatus> Expression { get; }
    
            public OrderStatusHandler(IReadRepository repository, IProjectionManager manager)
            {
                _repository = repository;

                Expression = manager.Create<OrderStatus>()
                    .From<Order>()
                    .ForEachStream()
                    .OutputState();
    
                 Expression.Invoke();
            }
    
            public OrderStatus Handle(OrderStatusQuery query)
            {
                OrderStatus orderStatus;

                if (query.OrderNumber != null)
                    orderStatus = _repository.Find<OrderStatus>(o => o.OrderNumber == query.OrderNumber);
                else
                    orderStatus = _repository.Find<Guid,OrderStatus>(query.OrderId);
                if(orderStatus == null)
                    throw new InvalidOperationException("Order info not found");

                return orderStatus;
            }
        }    
}