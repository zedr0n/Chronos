using System;
using Chronos.Core.Scheduling.Commands;
using Chronos.Core.Scheduling.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;
using NodaTime;

namespace Chronos.Core.Sagas
{
    public class SchedulerSagaHandler : SagaHandlerBase<SchedulerSaga>
    {
        private class SagaReplayStrategy : IReplayStrategy
        {
            public void Replay(Instant date)
            {
                throw new NotImplementedException();
            }

            public Func<IMessage, bool> Replayable => m => m is RequestStopAtCommand;
        }
        
        public SchedulerSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore) 
            : base(repository, debugLog, eventStore)
        {
            RegisterAlert<CommandSchedulingRequested>(e => e.ScheduleId,true);
            RegisterAlert<TimeoutRequested>(e => e.ScheduleId);
            RegisterAlert<TimeoutCompleted>(e => e.ScheduleId);
            RegisterAlert<StopRequested>(e => e.ScheduleId);
            RegisterAlert<StopCompleted>(e => e.ScheduleId);
            
            //ReplayStrategy = new SagaReplayStrategy();
        }
    }
}