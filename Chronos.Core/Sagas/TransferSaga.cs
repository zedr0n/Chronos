using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Transactions;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure.Events;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class TransferSaga : StatelessSaga<TransferSaga.STATE,TransferSaga.TRIGGER>,
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
            CASHTRANSFER_CREATED,
            ASSETTRANSFER_CREATED
        }

        private TransferDetails _transferDetails;
        private double _amount;
        private Guid _assetId;

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            StateMachine.Configure(STATE.OPEN)
                .Permit(TRIGGER.ASSETTRANSFER_CREATED, STATE.COMPLETED);

            StateMachine.Configure(STATE.COMPLETED)
                .OnEntryFrom(TRIGGER.ASSETTRANSFER_CREATED, TransferAsset)
                .OnEntryFrom(TRIGGER.CASHTRANSFER_CREATED, TransferCash)
                .OnEntry(OnComplete);
        }

        private void TransferCash()
        {
            SendMessage(new DepositCashCommand
            {
                AggregateId = _transferDetails.AccountTo,
                Amount = _amount
            });
            SendMessage(new WithdrawCashCommand
            {
                AggregateId = _transferDetails.AccountFrom,
                Amount = _amount
            });
        }
        private void TransferAsset()
        {
            SendMessage(new DepositAssetCommand
            {
                AggregateId = _transferDetails.AccountTo,
                AssetId = _assetId
            });
        }

        public void When(AssetTransferCreated e)
        {
            if (base.When(e))
                return;

            _transferDetails = new TransferDetails(e.FromAccount,e.ToAccount);
            _assetId = e.AssetId;

            StateMachine.Fire(TRIGGER.ASSETTRANSFER_CREATED);
        }

        public void When(CashTransferCreated e)
        {
            if (base.When(e))
                return;

            _transferDetails = new TransferDetails(e.FromAccount, e.ToAccount);
            _amount = e.Amount;

            StateMachine.Fire(TRIGGER.CASHTRANSFER_CREATED);
        }

        protected override bool IsComplete() => StateMachine.IsInState(STATE.COMPLETED);

    }
}