using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;

namespace Chronos.Core.Sagas
{
    public class OrderTrackingSagaHandler : AssetTrackingSagaHandler
    {
        public OrderTrackingSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore) : base(repository, debugLog, eventStore)
        {
            Register<OrderTrackingRequested,OrderTrackingSaga>(e => e.AssetId);    
            Register<OrderTrackingSaga>();
        }
    }
}