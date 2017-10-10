using System;
using System.Linq;
using Chronos.Infrastructure.Sagas;
using NodaTime;

namespace Chronos.Infrastructure
{
    public static class TypeExtensions
    {
        public static string SerializableName(this Type type)
        {
            var name = type.GenericTypeArguments.Aggregate(type.Name, (current, g) => current + g.Name);
            return name;
        }
    }
    
    
    public class StreamDetails
    {
        public string Name { get; }
        public string SourceType { get; set; }
        public Guid Key { get; set; }
        public int Version { get; set; } = -1;
        
        public Guid Timeline { get; set; }
        public int BranchVersion { get; set; }

        public bool IsBranch => Timeline == Guid.Empty;
        public bool IsSaga => Name.Contains("Saga");
        
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
            : this(sourceType.SerializableName(),key)
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