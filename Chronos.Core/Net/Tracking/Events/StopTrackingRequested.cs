using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Tracking.Events
{
    public class StopTrackingRequested : EventBase
    {
        public Guid? AssetId { get; set; }
    }
}