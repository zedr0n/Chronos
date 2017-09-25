using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class SchedulerSagaHandler : SagaHandlerBase<SchedulerSaga>
    {
        public SchedulerSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStoreSubscriptions eventStore) 
            : base(repository, debugLog, eventStore)
        {
            RegisterAlert<CommandScheduled>(e => e.ScheduleId,true);
            RegisterAlert<TimeoutCompleted>(e => e.ScheduleId);
        }
    }
}