using Chronos.Core.Net.Json.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class JsonSagaHandler<T> : SagaHandlerBase<JsonSaga<T>>
    {
        public JsonSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStoreSubscriptions eventStore) 
            : base(repository, debugLog, eventStore)
        {
            Register<JsonRequestTracked<T>>(e => e.RequestId);
            RegisterTransient<TimeoutCompleted>(e => e.ScheduleId);
        }
    }
}