using System;
using Chronos.Core.Net.Json.Commands;
using Chronos.Core.Net.Json.Events;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Sagas;
using NodaTime;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class JsonSaga<T> : StatelessSaga<JsonSaga<T>.STATE,JsonSaga<T>.TRIGGER>,
        IHandle<JsonRequestTracked<T>>,
        IHandle<TimeoutCompleted>,
        IHandle<JsonRequestCompleted>
    {
        public enum STATE
        {
            OPEN,
            ACTIVE,
            FETCHING,
            COMPLETED
        }
        public enum TRIGGER
        {
            TRACKING_REQUESTED,
            JSON_REQUESTED,
            JSON_RECEIVED,
            TERMINATE
        }

        private Guid _requestId;
        private int _updateInterval;
        
        private void SetUpdate()
        {
            if (_updateInterval <= 0)
                return;
            
            SendMessage(new RequestTimeoutCommand
            {
                TargetId = SagaId,
                Duration = Duration.FromSeconds(_updateInterval)
            });
        }

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            StateMachine.Configure(STATE.OPEN)
                .Permit(TRIGGER.TRACKING_REQUESTED, STATE.ACTIVE);
            
            StateMachine.Configure(STATE.ACTIVE)
                .Permit(TRIGGER.JSON_REQUESTED, STATE.FETCHING)
                .Permit(TRIGGER.TERMINATE, STATE.COMPLETED);

            StateMachine.Configure(STATE.FETCHING)
                .Permit(TRIGGER.JSON_RECEIVED, STATE.ACTIVE);
        }

        public void When(JsonRequestTracked<T> e)
        {
            _updateInterval = e.UpdateInterval;
            _requestId = e.RequestId;
            
            StateMachine.Fire(TRIGGER.TRACKING_REQUESTED);
               
            //if(_updateInterval > 0)
            SetUpdate();
            base.When(e);
        }
     
        public void When(TimeoutCompleted e)
        {
            if (!StateMachine.IsInState(STATE.ACTIVE))
            {
                SetUpdate();
                return;
            }

            StateMachine.Fire(TRIGGER.JSON_REQUESTED);
           
            SendMessage(new ExecuteRequestCommand<T>
            {
                TargetId = _requestId
            });
            
            SetUpdate();
            base.When(e);
        }

        public void When(JsonRequestCompleted e)
        {
            StateMachine.Fire(TRIGGER.JSON_RECEIVED);
            base.When(e);
        }
    }
}