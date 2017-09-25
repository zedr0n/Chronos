using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chronos.Persistence.Types
{
    public class Stream
    {
        public int HashId { get; set; }
        public string Name { get; set; }
        public string SourceType { get; set; }
        public Guid Key { get; set; }
        public int Version { get; set; }
        public List<Event> Events { get; } = new List<Event>();
        
        public Guid TimelineId { get; set; } = Guid.Empty;
        public int BranchVersion { get; set; }
    }
}