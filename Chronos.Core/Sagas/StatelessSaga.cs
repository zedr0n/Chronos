using System;
using Chronos.Infrastructure.Logging;
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

        protected StatelessSaga() { }
        protected StatelessSaga(Guid sagaId) : base(sagaId)
        {
        }

        protected void OnTransition(StateMachine<TState, TTrigger>.Transition transition)
        {
            DebugLog?.WriteLine("    " + transition.Source + " :: " + transition.Destination);
        }

        protected virtual void ConfigureStateMachine()
        {
            StateMachine.OnTransitioned(OnTransition);
        }
    }
}