using System;
using System.Diagnostics;
using Chronos.Core.Net.Json.Commands;
using Chronos.Core.Net.Json.Events;
using Chronos.Core.Nicehash.Commands;
using Chronos.Core.Nicehash.Events;
using Chronos.Core.Nicehash.Json;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class NicehashTrackingSaga : StatelessSaga<NicehashTrackingSaga.STATE, NicehashTrackingSaga.TRIGGER>,
        IHandle<NicehashOrderTrackingRequested>,
        IHandle<JsonRequestCompleted>,
        IHandle<OrderStatusParsed>
    {
        public enum STATE
        {
            OPEN,
            ACTIVE,
            UPDATING,
            COMPLETED
        }
        public enum TRIGGER
        {
            TRACKING_REQUESTED,
            ORDER_UPDATED,
            UPDATE_COMPLETED,
            ORDER_COMPLETED
        }

        private Guid _orderId;
        private int _orderNumber;
        private Guid _requestId;
        
        public NicehashTrackingSaga() {}

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            StateMachine.Configure(STATE.OPEN)
                .Permit(TRIGGER.TRACKING_REQUESTED, STATE.ACTIVE);
            
            StateMachine.Configure(STATE.ACTIVE)
                .Permit(TRIGGER.ORDER_UPDATED, STATE.UPDATING)
                .Permit(TRIGGER.ORDER_COMPLETED, STATE.COMPLETED);

            StateMachine.Configure(STATE.UPDATING)
                .Permit(TRIGGER.UPDATE_COMPLETED, STATE.ACTIVE);
            
            base.ConfigureStateMachine();

        }

        public void When(NicehashOrderTrackingRequested e)
        {
            Debug.Assert(SagaId == e.OrderId);
            _orderId = e.OrderId;
            _orderNumber = e.OrderNumber;

            // on first run
            if (_requestId == Guid.Empty)
            {
                _requestId = e.OrderId;
            
                SendMessage(new CreateRequestCommand<Orders>
                {
                    TargetId = _requestId
                }); 
            }

            SendMessage(new TrackRequestCommand<Orders>
            {
                TargetId = _requestId,
                UpdateInterval = e.UpdateInterval
            });
            
            StateMachine.Fire(TRIGGER.TRACKING_REQUESTED);
            base.When(e);
        }

        public void When(JsonRequestCompleted e)
        {
            SendMessage(new ParseOrderStatusCommand
            {
                TargetId = _orderId,
                RequestId = e.RequestId,
                OrderNumber = _orderNumber
            });
           
            base.When(e);
        }

        public void When(OrderStatusParsed e)
        {
            Debug.Assert(e.RequestId == _requestId);
            
            SendMessage(new UpdateOrderStatusCommand
            {
                Speed = e.Speed,
                Spent = e.Spent
            });
            
            StateMachine.Fire(TRIGGER.ORDER_UPDATED);
            base.When(e);
        }
        
        protected override void When(IEvent e)
        {
            When((dynamic) e);
        }

    }
}