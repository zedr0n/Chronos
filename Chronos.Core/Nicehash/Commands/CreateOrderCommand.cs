using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Nicehash.Commands
{
    public class CreateOrderCommand : CommandBase 
    {
        public int OrderNumber { get; set; }
        public Guid PriceAssetId { get; set; }
        public double Price { get; set; }
    }
}