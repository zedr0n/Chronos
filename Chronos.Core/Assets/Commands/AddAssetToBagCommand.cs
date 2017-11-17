using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class AddAssetToBagCommand : CommandBase
    {
        public Guid AssetId { get; }
        public double Quantity { get; }
    }
}