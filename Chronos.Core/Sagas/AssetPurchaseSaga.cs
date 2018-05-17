using System;
using System.Collections.Generic;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Assets.Commands;
using Chronos.Core.Assets.Events;
using Chronos.Core.Coinbase.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class AssetPurchaseSaga : StatelessSaga<AssetPurchaseSaga.State, AssetPurchaseSaga.Trigger>,
        IHandle<CoinPurchased>,
        IHandle<AssetPurchased>
    {
        private Guid _coinId;
        private double _quantity;
        private double _costPerUnit;
        private Guid _accountId;
        
        public enum State { Open, Processing, Completed }
        public enum Trigger { CoinPurchased, AssetPurchaseCreated }

        public override void When(IEvent e)
        {
            When((dynamic) e);
        }
        
        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<State, Trigger>(State.Open);

            StateMachine.Configure(State.Open)
                .Permit(Trigger.CoinPurchased, State.Processing);

            StateMachine.Configure(State.Processing)
                .Permit(Trigger.AssetPurchaseCreated, State.Completed);
            
            base.ConfigureStateMachine();
        }

        private void OnCompleted()
        {
            SendMessage(new PurchaseAssetCommand(_coinId,_quantity,_costPerUnit)
            {
                TargetId = SagaId
            });
        }

        
        public void When(CoinPurchased e)
        {
            if (StateMachine.CanFire(Trigger.CoinPurchased))
            {
                _coinId = e.CoinId;
                _quantity = e.Quantity;
                _costPerUnit = e.CostPerUnit;
                _accountId = e.AccountId;
                StateMachine.Fire(Trigger.CoinPurchased); 
            }
            
            base.When(e);
        }

        public void When(AssetPurchased e)
        {
            if (StateMachine.IsInState(State.Completed))
            {
                SendMessage(new DepositAssetCommand
                {
                    TargetId = _accountId,
                    Quantity = _quantity
                });
            }
            base.When(e);
        }
    }
}