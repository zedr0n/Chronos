using System;

namespace Chronos.Persistence.Types
{
    public class Command
    {
        public int Id { get; set; }
        public Guid Timeline { get; set; }
        
        public string Payload { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}