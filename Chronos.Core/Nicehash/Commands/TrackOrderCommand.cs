using System;
using Chronos.Core.Common.Commands;
using NodaTime;

namespace Chronos.Core.Nicehash.Commands
{
    public class TrackOrderCommand : TrackAssetCommand 
    {
        public int OrderNumber { get; set; }

        public TrackOrderCommand(Guid assetId, Duration updateInterval) : base(assetId, updateInterval)
        {
        }
    }
}