using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Coinbase.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class CoinbaseSaga : StatelessSaga<CoinbaseSaga.State,CoinbaseSaga.Trigger>,
        IHandle<CoinbaseAccountCreated>
    {
        public enum State { Open, Completed }
        public enum Trigger { AccountCreated }

        private Guid _accountId;
        
        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<State, Trigger>(State.Open);

            StateMachine.Configure(State.Open)
                .Permit(Trigger.AccountCreated,State.Completed);

            StateMachine.Configure(State.Completed)
                .OnEntry(OnCompleted);
            
            base.ConfigureStateMachine();
        }

        private void OnCompleted()
        {
            SendMessage(new CreateAccountCommand
            {
                TargetId = _accountId,
                Currency = "GBP",
                Name = "Coinbase"
            });
        }
        
        protected override void When(IEvent e) => When((dynamic) e);
        
        public void When(CoinbaseAccountCreated e)
        {
            if (StateMachine.IsInState(State.Open))
            {
                _accountId = e.AccountId;
                StateMachine.Fire(Trigger.AccountCreated);
            }
            base.When(e);
        }
    }
}