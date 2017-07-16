using System.Collections.Generic;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class SagaManagerBase : ISagaManager
    {
        private readonly ISagaRepository _repository;
        private readonly IDebugLog _debugLog;

        protected SagaManagerBase(ISagaRepository repository, IEventBus eventBus, IDebugLog debugLog)
        {
            _repository = repository;
            _debugLog = debugLog;
            this.RegisterAll(eventBus);
        }

        protected void When<TEvent, TSaga>(TEvent e) where TSaga : class, ISaga,new()
                                                     where TEvent : class, IEvent
        {
            var sagaId = e.SourceId;
            var saga = _repository.Find<TSaga>(sagaId) ?? new TSaga().LoadFrom<TSaga>(sagaId, new List<IEvent> () );
            saga.DebugLog = _debugLog;

            _debugLog.WriteLine(" -> " + saga.GetType().Name);

            saga.Dispatch(e);

            _repository.Save(saga);
        }
    }
}