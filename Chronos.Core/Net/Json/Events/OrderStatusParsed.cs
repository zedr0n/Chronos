using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Json.Events
{
    public class OrderStatusParsed : EventBase
    {
        public Guid RequestId { get; set; }
        public double Speed { get; set; }
        public double Spent { get; set; }
    }
}