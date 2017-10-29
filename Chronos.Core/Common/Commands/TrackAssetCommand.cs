using System;
using Chronos.Core.Net.Tracking;
using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Core.Common.Commands
{
    public class TrackAssetCommand : CommandBase
    {
        public Guid AssetId { get; }
        public Duration UpdateInterval { get; }

        public override Guid TargetId
        {
            get => Tracker.TrackerId;
            set { }
        }

        public TrackAssetCommand(Guid assetId, Duration updateInterval)
        {
            AssetId = assetId;
            UpdateInterval = updateInterval;
        }
    }
}