using System;

namespace Chronos.Infrastructure.Events
{
    public class JsonRequestCompleted : EventBase
    {
        public Guid RequestId { get; set; }
        public Guid RequestorId { get; set; }
    }
}