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

        public TransferSaga() { }
        public TransferSaga(Guid id) : base(id) { }

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            StateMachine.Configure(STATE.OPEN)
                .Permit(TRIGGER.ASSETTRANSFER_CREATED, STATE.COMPLETED)
                .Permit(TRIGGER.CASHTRANSFER_CREATED, STATE.COMPLETED);

            StateMachine.Configure(STATE.COMPLETED)
                .OnEntryFrom(TRIGGER.ASSETTRANSFER_CREATED, TransferAsset)
                .OnEntryFrom(TRIGGER.CASHTRANSFER_CREATED, TransferCash);

            base.ConfigureStateMachine();
        }

        private void TransferCash()
        {
            SendMessage(new DepositCashCommand
            {
                TargetId = _transferDetails.AccountTo,
                Amount = _amount
            });
            SendMessage(new WithdrawCashCommand
            {
                TargetId = _transferDetails.AccountFrom,
                Amount = _amount
            });
        }
        private void TransferAsset()
        {
            SendMessage(new DepositAssetCommand
            {
                TargetId = _transferDetails.AccountTo,
                AssetId = _assetId
            });
        }

        public void When(AssetTransferCreated e)
        {
            _transferDetails = new TransferDetails(e.FromAccount,e.ToAccount);
            _assetId = e.AssetId;

            StateMachine.Fire(TRIGGER.ASSETTRANSFER_CREATED);
            base.When(e);
        }

        public void When(CashTransferCreated e)
        {
            _transferDetails = new TransferDetails(e.FromAccount, e.ToAccount);
            _amount = e.Amount;

            StateMachine.Fire(TRIGGER.CASHTRANSFER_CREATED);
            base.When(e);
        }
    }
}