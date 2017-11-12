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
        
        private readonly IObservable<IEvent> _events;
        private readonly IObservable<IEvent> _alerts;

        private readonly ISagaEventHandler _eventHandler;

        protected IReplayStrategy ReplayStrategy { private get; set; }

        protected SagaHandlerBase(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore, ISagaEventHandler eventHandler)
        {
            _repository = repository;
            _debugLog = debugLog;
            _eventHandler = eventHandler;

            _alerts = eventStore.Alerts;//.Publish();
            //_alerts.Connect();

            _events = eventStore.Events;//.Publish();
            //_events.Connect();
        }
        
        private void Register<TEvent>(Action<TEvent> action) where TEvent : IEvent
        {
            _events.OfType<TEvent>().Subscribe(action);
        }

        protected void Register<TEvent, T>(Func<TEvent, Guid> sagaId, bool createNew = true)
            where TEvent : class,IEvent
            where T : class,TSaga, new() 
        {
            Register<TEvent>(e =>
                Send(e,
                    Get<T>(sagaId(e),createNew)));
        }
        
        protected void Register<TEvent>(Func<TEvent, Guid> sagaId,bool createNew = true)
            where TEvent : class,IEvent
        {
            Register<TEvent,TSaga>(sagaId,createNew);
        }

        private void RegisterAlert<TEvent>(Action<TEvent> action) where TEvent : IEvent
        {
            _alerts.OfType<TEvent>().Subscribe(action);
        }

        protected void RegisterAlert<TEvent>(Func<TEvent, Guid> sagaId, bool createNew = false) 
            where TEvent : class, IEvent
        {
            RegisterAlert<TEvent,TSaga>(sagaId,createNew);
        }
        
        protected void RegisterAlert<TEvent,T>(Func<TEvent, Guid> sagaId,bool createNew = false)
            where TEvent : class, IEvent
            where T : class, TSaga, new()
        {
            RegisterAlert<TEvent>(e => 
                Send(e,
                    Get<T>(sagaId(e),createNew)));
        }

        private T Get<T>(Guid sagaId, bool createNew = true)
            where T : class, ISaga, new()
        {
            var saga = _repository.Find<T>(sagaId,ReplayStrategy) ??
                       (createNew ? new T().LoadFrom<T>(sagaId, new List<IEvent> () ) : null);
            if (saga == null)
            {
                _debugLog.WriteLine("Saga " + sagaId + " not found");
                return null;
            }
            saga.DebugLog = _debugLog;

            //_debugLog.WriteLine("   -> " + saga.GetType().SerializableName());
            return saga;
        }

        public virtual void Send<TEvent,T>(TEvent e,T saga) where TEvent : class, IEvent
            where T : class, ISaga, new()
        {
            if (saga == null)
                return;
            _eventHandler.Handler(saga, e);
            _repository.Save(saga);
        }
    }
}