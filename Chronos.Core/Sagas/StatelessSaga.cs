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

        public override void When(IEvent e)
        {
            if (CanFire(e))
                Handle(e);   
            Fire(e);
            base.When(e);
        }

        private bool CanFire(IEvent e) 
        {
            return _triggers.ContainsKey(e.GetType());
        }

        private void Fire(IEvent e) 
        {
            if(CanFire(e))
                StateMachine.Fire(_triggers[e.GetType()]);
        }
        
        protected virtual void Handle(IEvent e) => When((dynamic) e); 
        
        protected void Register<TEvent>(TTrigger t) where TEvent : IEvent
        {
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