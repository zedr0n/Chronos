using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Tracking.Events
{
    public class JsonTrackingRequested : EventBase
    {
        public Guid RequestId { get; set; }
        public int UpdateInterval { get; set; }
    }
}