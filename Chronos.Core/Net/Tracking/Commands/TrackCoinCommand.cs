using System;
using NodaTime;

namespace Chronos.Core.Net.Tracking.Commands
{
    public class TrackCoinCommand : TrackAssetCommand
    {
        public string Ticker { get; set; }
        
        public TrackCoinCommand(Guid assetId, Duration updateInterval) : base(assetId, updateInterval)
        {
        }
    }
}