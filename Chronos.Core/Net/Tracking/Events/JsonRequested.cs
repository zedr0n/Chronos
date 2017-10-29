using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Tracking.Events
{
    public class JsonRequested : EventBase
    {
        public JsonRequested(string url, Guid requestorId)
        {
            Url = url;
            RequestorId = requestorId;
        }

        public string Url { get;  }
        public Guid RequestorId { get; }
    }
}