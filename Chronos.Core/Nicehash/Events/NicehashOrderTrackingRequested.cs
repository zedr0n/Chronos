using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Nicehash.Events
{
    public class NicehashOrderTrackingRequested : EventBase 
    {
        public Guid OrderId { get; set; }
        public int OrderNumber { get; set; }
        public int UpdateInterval { get; set; }
    }
}