using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Exchanges.Commands
{
    public class CreateExchangeOrderCommand : CommandBase
    {
        public CreateExchangeOrderCommand(Guid assetFrom, Guid assetTo, double quantityFrom, double quantityTo)
        {
            AssetFrom = assetFrom;
            AssetTo = assetTo;
            QuantityFrom = quantityFrom;
            QuantityTo = quantityTo;
        }

        public Guid AssetFrom { get; }
        public Guid AssetTo { get; }
        public double QuantityFrom { get; }
        public double QuantityTo { get; }
        
    }
}