using System;
using Chronos.Core.Net.Tracking.Commands;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Core.Scheduling.Commands;
using Chronos.Core.Scheduling.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;
using NodaTime;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class AssetTrackingSaga : StatelessSaga<AssetTrackingSaga.State,AssetTrackingSaga.Trigger>,
        IHandle<JsonRequestFailed>,
        IHandle<JsonReceived>,
        IHandle<AssetJsonParsed>,
        IHandle<TimeoutCompleted>,
        IHandle<StartRequested>,
        IHandle<StopTrackingRequested>
    {
        public enum State
        {
            Open,
            Active,
            Paused,
            Received
        }
        public enum Trigger
        {
            TrackingRequested,
            JsonReceived,
            Pause,
            Stop,
            Parsed
        }
        
        public AssetTrackingSaga()
        {
        }

        private string _url;
        private Duration _updateInterval;

        private StateMachine<State, Trigger>.TriggerWithParameters<AssetTrackingRequested> _trackingTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<string> _jsonReceivedTrigger;

        protected override void When(IEvent e)
        {
            When((dynamic) e);
        }

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<State,Trigger>(State.Open);

            _trackingTrigger = StateMachine.SetTriggerParameters<AssetTrackingRequested>(Trigger.TrackingRequested);
            _jsonReceivedTrigger = StateMachine.SetTriggerParameters<string>(Trigger.JsonReceived);

            StateMachine.Configure(State.Open)
                .Permit(Trigger.TrackingRequested, State.Paused)
                .Ignore(Trigger.Stop)
                .OnEntryFrom(Trigger.Stop,OnCancel);

            StateMachine.Configure(State.Paused)
                .Permit(Trigger.JsonReceived, State.Received)
                .OnEntryFrom(_trackingTrigger, OnTracking)
                .Permit(Trigger.Stop,State.Open)
                .Ignore(Trigger.Pause);

            StateMachine.Configure(State.Active)
                .Permit(Trigger.Pause, State.Paused)
                .Permit(Trigger.JsonReceived, State.Received)
                .Permit(Trigger.Stop, State.Open)
                .OnEntry(RequestTimeout);

            StateMachine.Configure(State.Received)
                .OnEntryFrom(_jsonReceivedTrigger, OnReceived)
                .PermitReentry(Trigger.JsonReceived)
                .Permit(Trigger.Parsed, State.Active)
                .Permit(Trigger.Stop, State.Open)
                .OnExit(OnParsed);

            base.ConfigureStateMachine();
        }

        protected void When(AssetTrackingRequested e)
        {
            StateMachine.Fire(_trackingTrigger,e);    
            base.When(e);
        }
        
        protected virtual void OnTracking(AssetTrackingRequested e)
        {
            _url = e.Url;
            _updateInterval = e.UpdateInterval;
        }

        private void OnCancel()
        {
            SendMessage(new CancelTimeoutCommand(SagaId));
        }
        
        public void When(StopTrackingRequested e)
        {
            StateMachine.Fire(Trigger.Stop);
            base.When(e);
        }
        
        public void When(StartRequested e)
        {
            SendMessage(new RequestJsonCommand(_url,SagaId));
        }

        private void RequestTimeout()
        {
            SendMessage(new RequestTimeoutCommand(SagaId, _updateInterval));
        }

        public void When(TimeoutCompleted e)
        {
            SendMessage(new RequestJsonCommand(_url,SagaId));       
        }

        public void When(JsonRequestFailed e)
        {
            StateMachine.Fire(Trigger.Pause);
            
            base.When(e);
        }

        public void When(JsonReceived e)
        {
            StateMachine.Fire(_jsonReceivedTrigger,e.Result);
            // do not store json
            base.When(new JsonReceived(e.Url,null,e.RequestorId));
        }

        protected virtual void OnReceived(string json) {}

        public void When(AssetJsonParsed e)
        {
            StateMachine.Fire(Trigger.Parsed);
            base.When(e);
        }

        protected virtual void OnParsed() {}
        

    }
}