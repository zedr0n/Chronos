using System;

namespace Chronos.Infrastructure.Events
{
    public class JsonRequested<T> : EventBase
    {
        public Guid RequestId { get; set; }
        public string Url { get; set; }
    }
}