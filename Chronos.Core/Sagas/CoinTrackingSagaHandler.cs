using Chronos.Core.Common.Events;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class CoinTrackingSagaHandler : AssetTrackingSagaHandler, ISagaHandler<CoinTrackingSaga>
    {
        public CoinTrackingSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore, ISagaEventHandler eventHandler) : base(repository, debugLog, eventStore, eventHandler)
        {
            Register<CoinTrackingRequested,CoinTrackingSaga>(e => e.AssetId);
            RegisterAlert<CoinInfoParsed,CoinTrackingSaga>(e => e.Id);
            Register<CoinTrackingSaga>();
        }
    }
}