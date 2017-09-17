using Chronos.Core.Net.Json.Events;
using Chronos.Core.Orders.NiceHash.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class NicehashTrackingSagaHandler : SagaHandlerBase<NicehashTrackingSaga>
    {
        public NicehashTrackingSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStoreSubscriptions eventStore) 
            : base(repository, debugLog, eventStore)
        {
            Register<NicehashOrderTrackingRequested>(e => e.OrderId);
            Register<JsonRequestCompleted>(e => e.RequestId); 
            RegisterAlert<OrderStatusParsed>(e => e.RequestId);
        }
    }
}