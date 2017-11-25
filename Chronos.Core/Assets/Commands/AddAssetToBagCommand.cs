﻿using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class AddAssetToBagCommand : CommandBase
    {
        public AddAssetToBagCommand(Guid assetId, double quantity)
        {
            AssetId = assetId;
            Quantity = quantity;
        }

        public Guid AssetId { get; }
        public double Quantity { get; }
    }
}