using System;
using Chronos.Core.Scheduling.Commands;
using Chronos.Core.Scheduling.Events;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using NodaTime;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class SchedulerSaga : StatelessSaga<SchedulerSaga.State,SchedulerSaga.Trigger>
    {
        public enum State { Open,Requesting,Pending,Completed }
        public enum Trigger { CommandScheduled, SchedulerActive, CommandDue }

        private ICommand _command;
        private Instant _when; 

        public SchedulerSaga()
        {
            Register<CommandSchedulingRequested>(Trigger.CommandScheduled, When); 
            Register<StopRequested>(Trigger.SchedulerActive);
            Register<StopCompleted>(Trigger.CommandDue);
        }

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<State, Trigger>(State.Open);

            StateMachine.Configure(State.Open)
                .Permit(Trigger.CommandScheduled, State.Requesting);

            StateMachine.Configure(State.Requesting)
                .OnEntry(() => 
                    SendMessage(new RequestStopAtCommand(SagaId,_when)
                    {
                        TargetId = SagaId
                    }))
                .Permit(Trigger.SchedulerActive, State.Pending)
                .Ignore(Trigger.CommandScheduled);

            StateMachine.Configure(State.Pending)
                .Permit(Trigger.CommandDue, State.Completed);

            StateMachine.Configure(State.Completed)
                .OnEntry(() => SendMessage(_command));

            base.ConfigureStateMachine();
        }

        private void When(CommandSchedulingRequested e)
        {
            _command = e.Command;
            _when = e.When;
        }
    }
}