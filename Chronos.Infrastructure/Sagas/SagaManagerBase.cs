using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;

namespace Chronos.Infrastructure.Sagas
{
    public class SagaManagerBase : ISagaManager
    {
        private readonly ISagaRepository _repository;
        private readonly IDebugLog _debugLog;

        private readonly IConnectableObservable<IEvent> _events;

        protected SagaManagerBase(ISagaRepository repository, IDebugLog debugLog, IEventStoreSubscriptions eventStore)
        {
            _repository = repository;
            _debugLog = debugLog;

            _events = eventStore.AggregateEvents.Publish();
            _events.Connect();
            //this.RegisterAll(eventBus);
        }

        protected void Register<TEvent>(Action<TEvent> action) where TEvent : IEvent
        {
            _events.OfType<TEvent>().Subscribe(action);
        }

        protected void When<TEvent, TSaga>(TEvent e, Func<TEvent,Guid> sagaId, bool createNew = true, Action<TSaga> action = null) where TSaga : class, ISaga, IHandle<TEvent>,new()
                                                     where TEvent : class, IEvent
        {
            var saga = _repository.Find<TSaga>(sagaId(e)) ??
                       (createNew ? new TSaga().LoadFrom<TSaga>(sagaId(e), new List<IEvent> () ) : null);
            if (saga == null)
                return;
            saga.DebugLog = _debugLog;

            _debugLog.WriteLine("   -> " + saga.GetType().Name);

            action?.Invoke(saga);

            saga.When(e);
            //saga.Dispatch(e);

            _repository.Save(saga);
        }
    }
}