using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;

namespace Chronos.Infrastructure.Sagas
{
    public class SagaHandlerBase<TSaga> : ISagaHandler<TSaga>
        where TSaga : class, ISaga, new()
    {
        private readonly ISagaRepository _repository;
        private readonly IDebugLog _debugLog;
        
        private readonly IConnectableObservable<IEvent> _events;
        private readonly IConnectableObservable<IEvent> _alerts;

        protected SagaHandlerBase(ISagaRepository repository, IDebugLog debugLog, IEventStoreSubscriptions eventStore)
        {
            _repository = repository;
            _debugLog = debugLog;

            _alerts = eventStore.Alerts.Publish();
            _alerts.Connect();
            
            _events = eventStore.Events.Publish();
            _events.Connect();
        }
        
        private void Register<TEvent>(Action<TEvent> action) where TEvent : IEvent
        {
            _events.OfType<TEvent>().Subscribe(action);
        }

        protected void Register<TEvent>(Func<TEvent, Guid> sagaId)
            where TEvent : class,IEvent
        {
            Register<TEvent>(e => 
                Send(e,
                    Get(sagaId(e))));
        }

        private void RegisterAlert<TEvent>(Action<TEvent> action) where TEvent : IEvent
        {
            _alerts.OfType<TEvent>().Subscribe(action);
        }

        protected void RegisterAlert<TEvent>(Func<TEvent, Guid> sagaId)
            where TEvent : class, IEvent
        {
            RegisterAlert<TEvent>(e => 
                Send(e,
                    Get(sagaId(e))));
        }

        private TSaga Get(Guid sagaId, bool createNew = true)
        {
            var saga = _repository.Find<TSaga>(sagaId) ??
                       (createNew ? new TSaga().LoadFrom<TSaga>(sagaId, new List<IEvent> () ) : null);
            if (saga == null)
                return null;
            saga.DebugLog = _debugLog;

            _debugLog.WriteLine("   -> " + saga.GetType().Name);
            return saga;
        }

        private void Send<TEvent>(TEvent e,TSaga saga) where TEvent : class, IEvent
        {
            (saga as IHandle<TEvent>)?.When(e);
            _repository.Save(saga); 
        }
        
    }
}