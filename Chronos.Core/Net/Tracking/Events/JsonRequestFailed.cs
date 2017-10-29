using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Tracking.Events
{
    public class JsonRequestFailed : EventBase
    {
        public JsonRequestFailed(string url, Guid requestorId)
        {
            Url = url;
            RequestorId = requestorId;
        }

        public Guid RequestorId { get; }
        public string Url { get; }
    }
}