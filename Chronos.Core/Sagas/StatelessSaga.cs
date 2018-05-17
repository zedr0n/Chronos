using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;
using Stateless;

namespace Chronos.Core.Sagas
{
    public abstract class StatelessSaga<TState,TTrigger> : SagaBase
    {
        protected StateMachine<TState, TTrigger> StateMachine
        {
            get
            {
                if(_stateMachine == null)
                    ConfigureStateMachine();
                return _stateMachine;
            }
            set => _stateMachine = value;
        }
        private StateMachine<TState, TTrigger> _stateMachine;
        private readonly Dictionary<Type, TTrigger> _triggers = new Dictionary<Type, TTrigger>();
        private readonly Dictionary<Type, Action<IEvent>> _handlers = new Dictionary<Type, Action<IEvent>>();

        public override void When(IEvent e)
        {
            if (_triggers.TryGetValue(e.GetType(), out var trigger))
            {
                if (_handlers.TryGetValue(e.GetType(), out var handler))
                    handler(e);
                StateMachine.Fire(trigger);
            }
            base.When(e);
        }

        protected void Register<TEvent>(TTrigger t, Action<TEvent> handler = null)
            where TEvent : class, IEvent
        {
            if (handler != null)
                _handlers[typeof(TEvent)] = e => handler(e as TEvent);
            _triggers[typeof(TEvent)] = t;
        }
        
        protected StatelessSaga() { }
        protected StatelessSaga(Guid sagaId) : base(sagaId)
        {
        }

        protected void OnTransition(StateMachine<TState, TTrigger>.Transition transition)
        {
            //DebugLog?.WriteLine("      " + transition.Source + " :: " + transition.Destination);
        }

        protected virtual void ConfigureStateMachine()
        {
            StateMachine.OnTransitioned(OnTransition);
        }
    }
}