using System;

namespace Chronos.Persistence
{
    public class Event
    {
        // EF key
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; }
        public int Version { get; set; }
        public string Payload { get; set; }
        public string CorrelationId { get; set; }
    }
}