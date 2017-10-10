using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Json.Events
{
    public class JsonRequestFailed : EventBase
    {
        public Guid RequestId { get; set; }
    }
}