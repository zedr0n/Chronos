using System;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Infrastructure
{
    public class StreamDetails
    {
        public string Name { get; }
        public string SourceType { get; set; }
        public Guid Key { get; set; }
        public int Version { get; set; } = -1;
        
        public Guid Timeline { get; set; }
        public int BranchVersion { get; set; }

        public bool IsBranch => Timeline == Guid.Empty;
        
        public StreamDetails(StreamDetails rhs)
        {
            Name = rhs.Name;
            SourceType = rhs.SourceType;
            Key = rhs.Key;
            Version = rhs.Version;
            Timeline = rhs.Timeline;
        }
        
        public StreamDetails(string name)
        {
            Name = name;
        }

        public StreamDetails(IAggregate aggregate)
            : this(aggregate.GetType(),aggregate.Id)
        { }

        public StreamDetails(ISaga saga)
            : this(saga.GetType(),saga.SagaId)
        { }

        public StreamDetails(Type sourceType, Guid key)
            : this(sourceType.Name,key)
        {
        }

        public StreamDetails(string sourceType, Guid key)
        {
            SourceType = sourceType;
            Key = key;
            Name = $"{SourceType}-{key}";
        }
    }


}