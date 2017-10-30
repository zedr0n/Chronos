using System;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Core.Net.Tracking.Events
{
    public class AssetTrackingRequested : EventBase
    {
        public Guid AssetId { get; set; }
        public Duration UpdateInterval { get; set; }
        public string Url { get; set; }
    }
}