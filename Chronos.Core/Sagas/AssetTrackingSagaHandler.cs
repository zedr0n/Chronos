using Chronos.Core.Net.Tracking.Events;
using Chronos.Core.Scheduling.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class AssetTrackingSagaHandler : SagaHandlerBase<AssetTrackingSaga>
    {
        public AssetTrackingSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore)
            : base(repository, debugLog, eventStore)
        {
            //Register<AssetTrackingRequested>(e => e.AssetId);

        }
        
        protected void Register<TSaga>()
            where TSaga : AssetTrackingSaga, new()
        {
            RegisterAlert<TimeoutCompleted,TSaga>(e => e.ScheduleId);
            RegisterAlert<JsonReceived,TSaga>(e => e.RequestorId);
            RegisterAlert<JsonRequestFailed,TSaga>(e => e.RequestorId);
            RegisterAlert<AssetJsonParsed,TSaga>(e => e.Id); 
        }
    }
}