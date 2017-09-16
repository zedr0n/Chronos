using System;

namespace Chronos.Infrastructure.Events
{
    public class JsonTrackingRequested<T> : EventBase
    {
        public Guid RequestId { get; set; }
        
        public string Url { get; set; }
        public int UpdateInterval { get; set; }
    }
}