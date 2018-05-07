using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Exchanges.Commands
{
    public class FillExchangeOrderCommand : CommandBase
    {
        public FillExchangeOrderCommand(Guid fromAssetId, Guid toAssetId, double fromQuantity, double toQuantity)
        {
            FromAssetId = fromAssetId;
            ToAssetId = toAssetId;
            FromQuantity = fromQuantity;
            ToQuantity = toQuantity;
        }

        public Guid FromAssetId { get; }
        public Guid ToAssetId { get; }
        public double FromQuantity { get; }
        public double ToQuantity { get; }
    }
}