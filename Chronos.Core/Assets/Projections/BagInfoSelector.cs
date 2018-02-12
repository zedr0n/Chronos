using System;
using System.Reactive.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.ProjectionServices;

namespace Chronos.Core.Assets.Projections
{
    public class BagInfoSelector : BaseStreamSelector<BagInfo>
    {
        public BagInfoSelector(IVersionProvider<BagInfo> versionProvider, IEventStore eventStore) : base()
        {
            var bagStream = typeof(Bag).SerializableName();
            var coinStream = typeof(Coin).SerializableName();

            var allStreams = eventStore.GetLiveStreams();

            Streams = allStreams.Where(s => s.SourceType == bagStream || s.SourceType == coinStream);
        }

        public override IObservable<StreamDetails> Streams { get; }
    }
}