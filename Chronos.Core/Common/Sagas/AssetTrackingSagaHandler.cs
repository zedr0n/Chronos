using Chronos.Core.Common.Events;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Core.Scheduling.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Common.Sagas
{
    public class AssetTrackingSagaHandler : SagaHandlerBase<AssetTrackingSaga>
    {
        public AssetTrackingSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore)
            : base(repository, debugLog, eventStore)
        {
            Register<AssetTrackingRequested>(e => e.AssetId);
            RegisterAlert<TimeoutCompleted>(e => e.ScheduleId);
            RegisterAlert<JsonReceived>(e => e.RequestorId);
            RegisterAlert<JsonRequestFailed>(e => e.RequestorId);
            RegisterAlert<AssetJsonParsed>(e => e.Id);
        }
    }
}