using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Coinbase.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class CoinbaseSaga : StatelessSaga<CoinbaseSaga.State,CoinbaseSaga.Trigger>
    {
        public enum State { Open, Completed }
        public enum Trigger { AccountCreated }

        private Guid _accountId;

        public CoinbaseSaga()
        {
            Register<CoinbaseAccountCreated>(Trigger.AccountCreated, When);
        }
        
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

        private void When(CoinbaseAccountCreated e)
        {
            _accountId = e.AccountId;
        }
    }
}
