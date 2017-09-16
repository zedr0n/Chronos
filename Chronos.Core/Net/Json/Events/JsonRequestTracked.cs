using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Json.Events
{
    public class JsonRequestTracked<T> : EventBase
    {
        public Guid RequestId { get; set; }
        public int UpdateInterval { get; set; }
    }
}