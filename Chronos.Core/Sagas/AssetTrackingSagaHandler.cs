using Chronos.Core.Net.Tracking.Events;
using Chronos.Core.Scheduling.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class AssetTrackingSagaHandler : SagaHandlerBase<AssetTrackingSaga>
    {
        protected AssetTrackingSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore, ISagaEventHandler eventHandler)
            : base(repository, debugLog, eventStore, eventHandler)
        {
        }
        
        protected void Register<TSaga>()
            where TSaga : AssetTrackingSaga, new()
        {
            RegisterAlert<TimeoutCompleted,TSaga>(e => e.ScheduleId);
            RegisterAlert<JsonReceived,TSaga>(e => e.RequestorId);
            RegisterAlert<JsonRequestFailed,TSaga>(e => e.RequestorId);
            Register<StartRequested, TSaga>(e => e.AssetId,false);
            //RegisterAlert<AssetJsonParsed,TSaga>(e => e.Id); 
        }
    }
}