using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Core.Orders.NiceHash.Commands;
using Chronos.Core.Orders.NiceHash.Events;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;
using NodaTime;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class NicehashOrderSaga : StatelessSaga<NicehashOrderSaga.STATE,NicehashOrderSaga.TRIGGER>,
        IHandle<TimeoutCompleted>

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
            ORDER_CREATED,
            ORDER_UPDATED,
            UPDATE_COMPLETED,
            ORDER_COMPLETED
        }

        private Guid _orderId;
        private int _orderNumber;
        
        public NicehashOrderSaga() {}

        private void UpdateOrder(Orders.NiceHash.JSON.ApiOrderStatus apiOrderStatus)
        {
            if (apiOrderStatus == null)
                throw new InvalidOperationException("Couldn't retrive order status");
            
            SendMessage(new UpdateOrderStatusCommand
            {
                Speed = apiOrderStatus.Accepted_Speed,
                Spent = apiOrderStatus.Btc_paid
            }); 
            
            StateMachine.Fire(TRIGGER.UPDATE_COMPLETED);
        }

        private void SetUpdate()
        {
            SendMessage(new RequestTimeoutCommand
            {
                TargetId = SagaId,
                Duration = Duration.FromSeconds(10)
            });
        }

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            StateMachine.Configure(STATE.OPEN)
                .Permit(TRIGGER.ORDER_CREATED, STATE.ACTIVE)
                .OnExit(SetUpdate);
            
            StateMachine.Configure(STATE.ACTIVE)
                .Permit(TRIGGER.ORDER_UPDATED, STATE.UPDATING)
                .Permit(TRIGGER.ORDER_COMPLETED, STATE.COMPLETED);

            StateMachine.Configure(STATE.UPDATING)
                .Permit(TRIGGER.UPDATE_COMPLETED, STATE.ACTIVE);

        }

        public void When(NicehashOrderCreated e)
        {
            _orderId = e.OrderId;
            _orderNumber = e.OrderNumber;
            
            StateMachine.Fire(TRIGGER.ORDER_CREATED);
            base.When(e);
        }

        public void When(TimeoutCompleted e)
        {            
            SendMessage(new RequestJSONCommand<List<Orders.NiceHash.JSON.ApiOrderStatus>>
            {
                TargetId = SagaId,
                Handler = orders => UpdateOrder(orders.SingleOrDefault(o => o.Id == _orderNumber))
            });
            StateMachine.Fire(TRIGGER.ORDER_UPDATED);
            
            SetUpdate();
            base.When(e);
        }
        
        protected override void When(IEvent e)
        {
            When((dynamic) e);
            //base.When(e);
        }
    }
}