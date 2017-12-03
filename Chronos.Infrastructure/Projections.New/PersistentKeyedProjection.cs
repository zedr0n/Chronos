namespace Chronos.Infrastructure.Projections.New
{
    public class PersistentKeyedProjection<T> : PersistentPartitionedProjection<T>
        where T : class, IReadModel, new()
    {
        private string _keyAggregateType;

        public string KeyAggregateType
        {
            set
            {
                _keyAggregateType = value;
                Key = new KeySelector(s => s.Key, t => t == _keyAggregateType);
            }   
        }

        protected override bool AddReset(StreamDetails stream)
        {
            return stream.SourceType == _keyAggregateType;
        }

        public PersistentKeyedProjection(IEventStore eventStore, IStateWriter writer, IReadRepository readRepository) 
            : base(eventStore, writer, readRepository)
        {
        }
    }
}