using System;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Sagas;
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

        public SchedulerSaga(Guid sagaId) : base(sagaId)
        {
        }

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
                .OnEntry(ExecuteCommand)
                .OnEntry(OnComplete);
        }

        private void ExecuteCommand()
        {
            SendMessage(_command);
        }

        private void RequestTimeout()
        {
            SendMessage(new TimeoutRequested
            {
                SourceId = SagaId,
                When = _scheduledOn
            });
        }

        protected override bool IsComplete()
        {
            return StateMachine.IsInState(STATE.COMPLETED);
        }

        public void When(CommandScheduled e)
        {
            if(!base.When(e))
                return;

            _command = e.Command;
            _scheduledOn = e.Time;

            StateMachine.Fire(TRIGGER.COMMAND_SCHEDULED);
        }


        public void When(TimeoutCompleted e)
        {
            if (!base.When(e))
                return;

            // remove the timeout request if any
            // there will be one when saga is rebuilt from events
            ClearUndispatchedMessages();

            StateMachine.Fire(TRIGGER.COMMAND_DUE);

        }
    }
}