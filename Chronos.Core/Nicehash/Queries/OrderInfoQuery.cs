using System;
using Chronos.Core.Nicehash.Projections;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Nicehash.Queries
{
    public class OrderInfoQuery : IQuery<OrderInfo>
    {
        public Guid OrderId { get; set; }
        public int? OrderNumber { get; set; }
    }
}