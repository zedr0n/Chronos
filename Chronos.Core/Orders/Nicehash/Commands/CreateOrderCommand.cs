using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Orders.NiceHash.Commands
{
    public class CreateOrderCommand : CommandBase 
    {
        public Guid OrderId { get; set; }
        public int OrderNumber { get; set; }
        public double Price { get; set; }
    }
}