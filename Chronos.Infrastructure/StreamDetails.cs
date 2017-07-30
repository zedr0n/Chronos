using System;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Infrastructure
{
    public class StreamDetails
    {
        public string Name { get; }
        public string SourceType { get; }
        public Guid Id { get; }

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

        public StreamDetails(Type sourceType, Guid id)
            : this(sourceType.Name,id)
        {
        }

        public StreamDetails(string sourceType, Guid id)
        {
            SourceType = sourceType;
            Id = id;
            Name = $"{SourceType}-{id}";
        }
    }


}