using System;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Infrastructure
{
    public class StreamDetails
    {
        public string Name { get; }
        public string SourceType { get; }
        public Guid Key { get; }

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