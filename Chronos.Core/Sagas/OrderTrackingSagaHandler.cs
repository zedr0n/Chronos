using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class OrderTrackingSagaHandler : AssetTrackingSagaHandler, ISagaHandler<OrderTrackingSaga>
    {
        public OrderTrackingSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore, ISagaEventHandler eventHandler) : base(repository, debugLog, eventStore, eventHandler)
        {
            Register<OrderTrackingRequested,OrderTrackingSaga>(e => e.AssetId);    
            Register<OrderTrackingSaga>();
        }
    }
}