using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;
using NodaTime;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class SchedulerSaga : StatelessSaga<SchedulerSaga.STATE,SchedulerSaga.TRIGGER>
        , IHandle<CommandScheduled>
        , IHandle<TimeoutCompleted>
        , IHandle<TimeoutRequested>
    {
        public enum STATE
        {
            OPEN,
            SCHEDULED,
            COMPLETED
        }

        public enum TRIGGER
        {
            COMMAND_SCHEDULED,
            COMMAND_DUE
        }

        private ICommand _command;
        private Instant _scheduledOn;
        
        public SchedulerSaga() { }

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<STATE, TRIGGER>(STATE.OPEN);

            StateMachine.Configure(STATE.OPEN)
                .Permit(TRIGGER.COMMAND_SCHEDULED, STATE.SCHEDULED);

            StateMachine.Configure(STATE.SCHEDULED)
                .Ignore(TRIGGER.COMMAND_SCHEDULED)
                .Permit(TRIGGER.COMMAND_DUE, STATE.COMPLETED);

            StateMachine.Configure(STATE.COMPLETED)
                .Ignore(TRIGGER.COMMAND_SCHEDULED)
                .Ignore(TRIGGER.COMMAND_DUE)
                .OnEntry(ExecuteCommand);

            base.ConfigureStateMachine();
        }

        private void ExecuteCommand()
        {
            SendMessage(_command);
        }

        private void RequestTimeout()
        {
            SendMessage(new RequestTimeoutCommand
            {
                TargetId = SagaId,
                When = _scheduledOn
            });
        }

        public void When(CommandScheduled e)
        {
            _command = e.Command;
            _scheduledOn = e.Time;
            
            RequestTimeout();
            //StateMachine.Fire(TRIGGER.COMMAND_SCHEDULED);
            base.When(e);
        }

        public void When(TimeoutRequested e)
        {
            StateMachine.Fire(TRIGGER.COMMAND_SCHEDULED);
            base.When(e);
        }
        
        public void When(TimeoutCompleted e)
        {
            if (StateMachine.IsInState(STATE.COMPLETED))
                return;
            
            StateMachine.Fire(TRIGGER.COMMAND_DUE);
            base.When(e);
        }

        protected override void When(IEvent e)
        {
            When((dynamic) e);
            //base.When(e);
        }
    }
}