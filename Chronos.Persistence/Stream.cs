using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chronos.Persistence
{
    public class Stream
    {
        [Key]
        public int HashId { get; set; }
        public string Name { get; set; }
        public string SourceType { get; set; }
        public int Version { get; set; }
        public List<Event> Events { get; } = new List<Event>();
    }
}