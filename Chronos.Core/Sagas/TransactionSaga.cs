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
    public class TransactionSaga : SagaBase,
        IConsumer<PurchaseCreated>,
        IConsumer<SagaCompleted>
    {
        private enum STATE
        {
            OPEN,
            SCHEDULED,
            COMPLETED
        }

        private enum TRIGGER
        {
            TRANSACTION_CREATED,
            TRANSACTION_DUE
        }

        private StateMachine<STATE, TRIGGER> StateMachine {
            get
            {
                ConfigureStateMachine();
                return _stateMachine;
            }   
        }
        private StateMachine<STATE, TRIGGER> _stateMachine;

        private double _amount;
        private bool _purchaseCreated;
        private Guid _accountId;

        protected override bool IsComplete() => StateMachine.IsInState(STATE.COMPLETED);
        private void ConfigureStateMachine()
        {
            if (_stateMachine != null)
                return;

            _stateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            _stateMachine.Configure(STATE.OPEN)
                .PermitIf(TRIGGER.TRANSACTION_CREATED, STATE.COMPLETED, () => _purchaseCreated);

            _stateMachine.Configure(STATE.COMPLETED)
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

        public void When(SagaCompleted e)
        {
            OnCompletion();
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