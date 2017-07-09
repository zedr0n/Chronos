using System;
using Chronos.Core.Accounts.Events;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
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
        private readonly ISagaRepository _repository;

        private double _amount;
        private Guid _accountId;

        public TransactionSaga(Guid id, ISagaRepository repository) : base(id)
        {
            _repository = repository;
            _stateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            _stateMachine.Configure(STATE.OPEN)
                .Permit(TRIGGER.TRANSACTION_CREATED, STATE.SCHEDULED);

            _stateMachine.Configure(STATE.SCHEDULED)
                .OnEntry(Schedule)
                .Permit(TRIGGER.TRANSACTION_DUE, STATE.COMPLETED);

            _stateMachine.Configure(STATE.COMPLETED)
                .OnEntry(DebitAccount);
        }

        private void DebitAccount()
        {

        }

        public void When(PurchaseCreated e)
        {
            var sagaId = e.SourceId;
            var saga = _repository.Get<TransactionSaga>(sagaId);
            saga.Transition(e);
            _repository.Save(saga);
        }

        private void Schedule()
        {
            _stateMachine.Fire(TRIGGER.TRANSACTION_DUE);
        }

        private void Transition(PurchaseCreated e)
        {
            _amount = e.Amount;
            _accountId = e.AccountId;

            _stateMachine.Fire(TRIGGER.TRANSACTION_CREATED);

            PendingEvents.Add(e);
        }
    }
}