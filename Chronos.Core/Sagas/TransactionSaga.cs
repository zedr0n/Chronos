using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Events;
using Chronos.Core.Transactions.Commands;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Sagas;
using NodaTime;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class TransactionSaga : StatelessSaga<TransactionSaga.STATE, TransactionSaga.TRIGGER>,
        IConsumer<PurchaseCreated>,
        IConsumer<AssetTransferCreated>,
        IConsumer<CashTransferCreated>
        {
        public enum STATE
        {
            OPEN,
            COMPLETED
        }
        public enum TRIGGER
        {
            PURCHASE_CREATED,
            CASHTRANSFER_CREATED,
            ASSETTRANSFER_CREATED
        }

        private double _amount;
        private Guid _accountId;
        private Guid _toAccountId;
        private Guid _assetId;

        protected override bool IsComplete() => StateMachine.IsInState(STATE.COMPLETED);

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            StateMachine.Configure(STATE.OPEN)
                .Permit(TRIGGER.PURCHASE_CREATED, STATE.COMPLETED);
            StateMachine.Configure(STATE.OPEN)
                .Permit(TRIGGER.ASSETTRANSFER_CREATED, STATE.COMPLETED);

            StateMachine.Configure(STATE.COMPLETED)
                .OnEntryFrom(TRIGGER.ASSETTRANSFER_CREATED, TransferAsset)
                .OnEntryFrom(TRIGGER.CASHTRANSFER_CREATED, TransferCash)
                .OnEntryFrom(TRIGGER.PURCHASE_CREATED, WithdrawCash)
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

        private void TransferCash()
        {
            SendMessage(new DepositCashCommand
            {
                AggregateId = _toAccountId,
                Amount = _amount
            });
            SendMessage(new WithdrawCashCommand
            {
                AggregateId = _accountId,
                Amount = _amount
            });
        }
        private void TransferAsset()
        {
            SendMessage(new DepositAssetCommand
            {
                AggregateId = _toAccountId,
                AssetId = _assetId             
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
        public void When(AssetTransferCreated e)
        {
            if (base.When(e))
                return;

            _accountId = e.FromAccount;
            _assetId = e.AssetId;
            _toAccountId = e.ToAccount;

            StateMachine.Fire(TRIGGER.ASSETTRANSFER_CREATED);
        }
        public void When(CashTransferCreated e)
        {
            if (base.When(e))
                return;

            _accountId = e.FromAccount;
            _amount = e.Amount;

            StateMachine.Fire(TRIGGER.CASHTRANSFER_CREATED);
        }
    }
}