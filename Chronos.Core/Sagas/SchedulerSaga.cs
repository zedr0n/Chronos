using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using NodaTime;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class SchedulerSaga : StatelessSaga<SchedulerSaga.STATE,SchedulerSaga.TRIGGER>
        , IConsumer<CommandScheduled>
        , IConsumer<TimeoutCompleted>
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
                .Permit(TRIGGER.COMMAND_DUE, STATE.COMPLETED)
                .OnEntry(RequestTimeout);

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
                AggregateId = SagaId,
                When = _scheduledOn
            });
        }

        public void When(CommandScheduled e)
        {
            _command = e.Command;
            _scheduledOn = e.Time;

            StateMachine.Fire(TRIGGER.COMMAND_SCHEDULED);
            base.When(e);
        }

        public void When(TimeoutCompleted e)
        {
            StateMachine.Fire(TRIGGER.COMMAND_DUE);
            base.When(e);
        }
    }
}