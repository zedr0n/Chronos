using System;
using System.Collections.Generic;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Events;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Sagas;
using NodaTime;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class TransactionSaga : StatelessSaga<TransactionSaga.STATE, TransactionSaga.TRIGGER>,
        IConsumer<PurchaseCreated>
        {
        public enum STATE
        {
            OPEN,
            COMPLETED
        }

        public enum TRIGGER
        {
            TRANSACTION_CREATED
        }

        private double _amount;
        private bool _purchaseCreated;
        private Guid _accountId;

        protected override bool IsComplete() => StateMachine.IsInState(STATE.COMPLETED);

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            StateMachine.Configure(STATE.OPEN)
                .PermitIf(TRIGGER.TRANSACTION_CREATED, STATE.COMPLETED, () => _purchaseCreated);

            StateMachine.Configure(STATE.COMPLETED)
                .Ignore(TRIGGER.TRANSACTION_CREATED)
                .OnEntry(DebitAccount)
                .OnEntry(OnComplete);
        }

        public TransactionSaga(Guid id) : base(id)
        {
        }

        public TransactionSaga(Guid id, IEnumerable<IEvent> pastEvents)
            : base(id,pastEvents) { }

        private void DebitAccount()
        {
            SendMessage(new DebitAmountCommand
            {
                AggregateId = _accountId,
                Amount = _amount
            });
        }

        public void When(PurchaseCreated e)
        {
            if(!base.When(e))
                return;

            _amount = e.Amount;
            _purchaseCreated = true;
            _accountId = e.AccountId;

            StateMachine.Fire(TRIGGER.TRANSACTION_CREATED);
        }
    }
}