using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class CreateAssetExchangeCommand : CommandBase
    {
        public CreateAssetExchangeCommand(Guid assetFrom, Guid assetTo, double quantityFrom, double quantityTo)
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