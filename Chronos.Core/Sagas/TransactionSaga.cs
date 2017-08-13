using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure.Sagas;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class TransactionSaga : StatelessSaga<TransactionSaga.STATE, TransactionSaga.TRIGGER>,
        IHandle<PurchaseCreated>
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

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            StateMachine.Configure(STATE.OPEN)
                .Permit(TRIGGER.PURCHASE_CREATED, STATE.COMPLETED);

            StateMachine.Configure(STATE.COMPLETED)
                .Ignore(TRIGGER.PURCHASE_CREATED)
                .OnEntry(WithdrawCash);

            base.ConfigureStateMachine();
        }

        public TransactionSaga() { }

        private void WithdrawCash()
        {
            SendMessage(new WithdrawCashCommand
            {
                TargetId = _accountId,
                Amount = _amount
            });
        }

        public void When(PurchaseCreated e)
        {
            _amount = e.Amount;
            _accountId = e.AccountId;

            StateMachine.Fire(TRIGGER.PURCHASE_CREATED);
            base.When(e);
        }
    }
}