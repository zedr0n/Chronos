using Chronos.Core.Scheduling.Commands;
using Chronos.Core.Scheduling.Events;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class SchedulerSaga : StatelessSaga<SchedulerSaga.State,SchedulerSaga.Trigger>,
        IHandle<CommandSchedulingRequested>,
        IHandle<StopRequested>,
        IHandle<StopCompleted>
    {
        public enum State { Open,Requesting,Pending,Completed }
        public enum Trigger { CommandScheduled, SchedulerActive, CommandDue }

        private ICommand _command;

        private StateMachine<State, Trigger>.TriggerWithParameters<CommandSchedulingRequested> _commandTrigger;
        
        public SchedulerSaga() { }

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<State, Trigger>(State.Open);

            StateMachine.Configure(State.Open)
                .Permit(Trigger.CommandScheduled, State.Requesting);

            _commandTrigger = StateMachine.SetTriggerParameters<CommandSchedulingRequested>(Trigger.CommandScheduled);

            StateMachine.Configure(State.Requesting)
                .OnEntryFrom(_commandTrigger, OnScheduled)
                .Permit(Trigger.SchedulerActive, State.Pending)
                .Ignore(Trigger.CommandScheduled);

            StateMachine.Configure(State.Pending)
                .Ignore(Trigger.CommandScheduled)
                .Permit(Trigger.CommandDue, State.Completed);

            StateMachine.Configure(State.Completed)
                .OnEntry(ExecuteCommand);

            base.ConfigureStateMachine();
        }

        private void ExecuteCommand()
        {
            SendMessage(_command);
        }

        public void When(CommandSchedulingRequested e)
        {
            StateMachine.Fire(_commandTrigger,e);
            base.When(e);
        }

        private void OnScheduled(CommandSchedulingRequested e)
        {
            _command = e.Command;
            
            SendMessage(new RequestStopAtCommand(e.ScheduleId,e.When)
            {
                TargetId = SagaId
            }); 
        }

        public void When(StopRequested e)
        {
            StateMachine.Fire(Trigger.SchedulerActive);
            base.When(e);
        }
        
        public void When(StopCompleted e)
        {
            StateMachine.Fire(Trigger.CommandDue);
            base.When(e);
        }

        public override void When(IEvent e) => When((dynamic) e);
    }
}