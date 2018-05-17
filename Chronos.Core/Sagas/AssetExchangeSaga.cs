using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Assets.Commands;
using Chronos.Core.Assets.Events;
using Chronos.Core.Exchanges.Events;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class AssetExchangeSaga : StatelessSaga<AssetExchangeSaga.State,AssetExchangeSaga.Trigger>
        , IHandle<ExchangeAdded>, IHandle<ExchangeOrderFilled>,IHandle<AssetExchanged>
    {
        public enum State { Open, Exchanging, Filled }
        public enum Trigger { ExchangeAdded, OrderCreated, OrderFilled, AssetExchanged }

        private Guid _accountId;
        
        protected override void ConfigureStateMachine()
        {
            StateMachine.Configure(State.Open)
                .Permit(Trigger.ExchangeAdded, State.Exchanging);
            StateMachine.Configure(State.Exchanging)
                .Permit(Trigger.OrderCreated, State.Exchanging)
                .Permit(Trigger.OrderFilled, State.Filled);
            StateMachine.Configure(State.Filled)
                .Permit(Trigger.AssetExchanged, State.Exchanging);
            base.ConfigureStateMachine();
        }

        public void When(ExchangeAdded e)
        {
            if (StateMachine.IsInState(State.Open))
            {
                _accountId = e.ExchangeId;
                SendMessage(new CreateAccountCommand
                {
                    TargetId = _accountId,
                    Name = e.Name
                });
                
                StateMachine.Fire(Trigger.ExchangeAdded); 
            }

            base.When(e);    
        }
        
        public void When(ExchangeOrderFilled e)
        {
            StateMachine.Fire(Trigger.OrderFilled);
            if (StateMachine.IsInState(State.Filled))
            {
                SendMessage(new CreateAssetExchangeCommand(e.FromAsset, e.ToAsset, e.FromQuantity, e.ToQuantity, e.ExchangeId)
                {
                    TargetId = Guid.NewGuid()
                });
            }
            base.When(e);
        }
        
        public void When(AssetExchanged e)
        {
            if (StateMachine.IsInState(State.Filled))
            {
                SendMessage(new WithdrawAssetCommand
                {
                    AssetId = e.FromAsset,
                    TargetId = _accountId
                });
                SendMessage(new DepositAssetCommand
                {
                    AssetId = e.ToAsset,
                    TargetId = _accountId
                });
                StateMachine.Fire(Trigger.AssetExchanged);
            }
            base.When(e);
        }
    }
}