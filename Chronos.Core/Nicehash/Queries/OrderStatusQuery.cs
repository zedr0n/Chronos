using System;
using Chronos.Core.Nicehash.Projections;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Nicehash.Queries
{
    public class OrderStatusQuery : IQuery<OrderStatus>
    {
        public Guid OrderId { get; set; }
        public int? OrderNumber { get; set; }
    }
}