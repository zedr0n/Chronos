using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Transactions;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class TransferSaga : StatelessSaga<TransferSaga.State,TransferSaga.Trigger>,
        IHandle<AssetTransferCreated>,
        IHandle<CashTransferCreated>
    {
        public enum State { Open, Completed }
        public enum Trigger { CashTransferCreated, AssetTransferCreated }

        private TransferDetails _transferDetails;
        private double _amount;
        private Guid _assetId;

        public TransferSaga() { }

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<State, Trigger>(State.Open);

            StateMachine.Configure(State.Open)
                .Permit(Trigger.AssetTransferCreated, State.Completed)
                .Permit(Trigger.CashTransferCreated, State.Completed);

            StateMachine.Configure(State.Completed)
                .OnEntryFrom(Trigger.AssetTransferCreated, TransferAsset)
                .OnEntryFrom(Trigger.CashTransferCreated, TransferCash);

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

        public override void When(IEvent e) => When((dynamic) e);
        
        public void When(AssetTransferCreated e)
        {
            _transferDetails = new TransferDetails(e.FromAccount,e.ToAccount);
            _assetId = e.AssetId;

            StateMachine.Fire(Trigger.AssetTransferCreated);
            base.When(e);
        }

        public void When(CashTransferCreated e)
        {
            _transferDetails = new TransferDetails(e.FromAccount, e.ToAccount);
            _amount = e.Amount;

            StateMachine.Fire(Trigger.CashTransferCreated);
            base.When(e);
        }
    }
}