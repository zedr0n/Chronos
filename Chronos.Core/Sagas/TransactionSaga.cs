using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure.Sagas;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class TransactionSaga : StatelessSaga<TransactionSaga.State, TransactionSaga.Trigger>,
        IHandle<PurchaseCreated>
    {
        public enum State { Open, Completed }
        public enum Trigger { PurchaseCreated }

        private double _amount;
        private Guid _accountId;

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<State, Trigger>(State.Open);

            StateMachine.Configure(State.Open)
                .Permit(Trigger.PurchaseCreated, State.Completed);

            StateMachine.Configure(State.Completed)
                .Ignore(Trigger.PurchaseCreated)
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

            StateMachine.Fire(Trigger.PurchaseCreated);
            base.When(e);
        }
    }
}