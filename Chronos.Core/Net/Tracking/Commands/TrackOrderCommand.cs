using System;
using NodaTime;

namespace Chronos.Core.Net.Tracking.Commands
{
    public class TrackOrderCommand : TrackAssetCommand 
    {
        public int OrderNumber { get; set; }

        public TrackOrderCommand(Guid assetId, Duration updateInterval) : base(assetId, updateInterval)
        {
        }
    }
}