using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chronos.Persistence.Types
{
    public class Stream
    {
        [Key]
        public int HashId { get; set; }
        public string Name { get; set; }
        public string SourceType { get; set; }
        public Guid Key { get; set; }
        public int Version { get; set; }
        public List<Event> Events { get; } = new List<Event>();
    }
}