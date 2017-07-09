using System;
using System.Collections.Generic;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Events;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Sagas;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class TransactionSaga : SagaBase, IConsumer<PurchaseCreated>
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

        private readonly StateMachine<STATE, TRIGGER> _stateMachine;

        private double _amount;
        private bool _purchaseCreated;
        private Guid _accountId;

        public TransactionSaga(Guid id) : base(id)
        {
            _stateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            _stateMachine.Configure(STATE.OPEN)
                .PermitIf(TRIGGER.TRANSACTION_CREATED, STATE.COMPLETED, () => _purchaseCreated);

            _stateMachine.Configure(STATE.COMPLETED)
                .OnEntry(DebitAccount);
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
            _amount = e.Amount;
            _purchaseCreated = true;
            _accountId = e.AccountId;

            _stateMachine.Fire(TRIGGER.TRANSACTION_CREATED);
            base.When(e);
        }
    }
}