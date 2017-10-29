using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Tracking.Events
{
    public class JsonReceived : EventBase
    {
        public JsonReceived(string url, string result, Guid requestorId)
        {
            Url = url;
            Result = result;
            RequestorId = requestorId;
        }

        public Guid RequestorId { get; }
        public string Url { get; }
        public string Result { get; }
        
    }
}