﻿using System;
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
    public class AssetPurchaseSaga : StatelessSaga<AssetPurchaseSaga.State, AssetPurchaseSaga.Trigger>
    {
        private Guid _coinId;
        private double _quantity;
        private double _costPerUnit;
        private Guid _accountId;
        
        public enum State { Open, Processing, Completed }
        public enum Trigger { CoinPurchased, AssetPurchaseCreated }

        public AssetPurchaseSaga()
        {
            Register<CoinPurchased>(Trigger.CoinPurchased, When);
            Register<AssetPurchased>(Trigger.AssetPurchaseCreated);
        }
        
        private ICommand PurchaseCommand =>
            new PurchaseAssetCommand(_coinId, _quantity, _costPerUnit)
            {
                TargetId = SagaId
            };

        private ICommand DepositCommand =>
            new DepositAssetCommand
            {
                TargetId = _accountId,
                Quantity = _quantity
            };

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<State, Trigger>(State.Open);

            StateMachine.Configure(State.Open)
                .Permit(Trigger.CoinPurchased, State.Processing);

            StateMachine.Configure(State.Processing)
                .Permit(Trigger.AssetPurchaseCreated, State.Completed)
                .OnEntry(() => SendMessage(PurchaseCommand));

            StateMachine.Configure(State.Completed)
                .OnEntry(() => SendMessage(DepositCommand));
            
            base.ConfigureStateMachine();
        }

        private void When(CoinPurchased e)
        {
            _coinId = e.CoinId;
            _quantity = e.Quantity;
            _costPerUnit = e.CostPerUnit;
            _accountId = e.AccountId;
        }
    }
}