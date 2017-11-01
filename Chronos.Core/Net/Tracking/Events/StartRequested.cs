using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Tracking.Events
{
    public class StartRequested : EventBase
    {
        public StartRequested(Guid assetId)
        {
            AssetId = assetId;
        }

        public Guid AssetId { get; }
    }
}