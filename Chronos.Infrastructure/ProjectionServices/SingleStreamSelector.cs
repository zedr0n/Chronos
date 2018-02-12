using System;
using System.Reactive.Linq;
using System.Reflection;

namespace Chronos.Infrastructure.ProjectionServices
{
    public class SingleStreamSelector<T> : BaseStreamSelector<T> where T : class, IReadModel
    {
        public SingleStreamSelector(IEventStore eventStore)
        {
            var sourceType = typeof(T).GetTypeInfo().GetCustomAttribute<SourceAttribute>();
            if(sourceType == null)
                throw new InvalidOperationException("The read model should have source type specified");
            
            Streams = eventStore.GetLiveStreams().Where(s => s.SourceType == sourceType.SerializableName);
        }

        public override IObservable<StreamDetails> Streams { get; }
    }
}