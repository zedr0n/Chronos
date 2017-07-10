using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;
using Stateless;

namespace Chronos.Infrastructure.Sagas
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


        protected StatelessSaga(Guid sagaId) : base(sagaId)
        {
        }

        protected StatelessSaga(Guid id, IEnumerable<IEvent> pastEvents) : base(id, pastEvents)
        {
        }

        protected abstract void ConfigureStateMachine();

        protected abstract override bool IsComplete();
    }
}