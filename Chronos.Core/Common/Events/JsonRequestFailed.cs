using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Common.Events
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