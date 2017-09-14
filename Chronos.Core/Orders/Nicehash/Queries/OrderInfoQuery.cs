using System;
using Chronos.Core.Orders.NiceHash.Projections;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Orders.NiceHash.Queries
{
    public class OrderInfoQuery : IQuery<OrderInfo>
    {
        public Guid OrderId { get; set; }
        public int? OrderNumber { get; set; }
    }
}