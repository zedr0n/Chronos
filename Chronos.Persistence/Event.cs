using System;
using System.ComponentModel.DataAnnotations;

namespace Chronos.Persistence
{
    public class Event
    {
        public Guid Guid { get; set; }
        public DateTime Timestamp { get; set; }
        public string Payload { get; set; }
        [Key]
        public int EventNumber { get; set; }
    }
}