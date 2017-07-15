using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure.Events;
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
            PURCHASE_CREATED
        }

        private double _amount;
        private Guid _accountId;

        protected override bool IsComplete() => StateMachine.IsInState(STATE.COMPLETED);

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            StateMachine.Configure(STATE.OPEN)
                .Permit(TRIGGER.PURCHASE_CREATED, STATE.COMPLETED);

            StateMachine.Configure(STATE.COMPLETED)
                .OnEntry(WithdrawCash)
                .OnEntry(OnComplete);
        }

        public TransactionSaga() { }

        public TransactionSaga(Guid id) : base(id)
        {
        }

       private void WithdrawCash()
        {
            SendMessage(new WithdrawCashCommand
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
            _accountId = e.AccountId;

            StateMachine.Fire(TRIGGER.PURCHASE_CREATED);
        }
    }
}