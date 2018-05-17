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
        IHandle<JsonRequested>,
        IHandle<JsonReceived>,
        IHandle<AssetJsonParsed>,
        IHandle<TimeoutCompleted>,
        IHandle<StartRequested>,
        IHandle<StopTrackingRequested>
    {
        public enum State
        {
            Open,
            Requesting,
            Waiting,
            Received
        }
        public enum Trigger
        {
            TrackingRequested,
            Start,
            JsonRequested,
            JsonReceived,
            Pause,
            Stop,
            Parsed
        }
        
        private string _url;
        private Duration _updateInterval;
        private string _json;

        public override void When(IEvent e)
        {
            if (CanFire(e))
                Handle(e);   
            Fire(e);
            base.When(e);
        }

        protected virtual void Handle(IEvent e) => When((dynamic) e); 

        public AssetTrackingSaga()
        {
            Register<StopTrackingRequested>(Trigger.Stop);
            Register<StartRequested>(Trigger.Start);
            Register<JsonRequested>(Trigger.JsonRequested);
            Register<JsonReceived>(Trigger.JsonReceived);
            Register<JsonRequestFailed>(Trigger.Pause);
            Register<TimeoutCompleted>(Trigger.Start);  
        }

        protected override void ConfigureStateMachine()
        {
            StateMachine = new StateMachine<State,Trigger>(State.Open);

            StateMachine.Configure(State.Open)
                .Permit(Trigger.TrackingRequested, State.Waiting)
                .Ignore(Trigger.Stop)
                .OnEntryFrom(Trigger.Stop, () =>
                    SendMessage(new CancelTimeoutCommand(SagaId)));

            StateMachine.Configure(State.Waiting)
                .Permit(Trigger.JsonReceived, State.Received)
                .Permit(Trigger.Start, State.Requesting)
                .Permit(Trigger.Stop, State.Open)
                .Ignore(Trigger.Pause);

            StateMachine.Configure(State.Requesting)
                .Permit(Trigger.JsonRequested, State.Waiting)
                .OnEntry(() =>
                    SendMessage(new RequestJsonCommand(_url, SagaId)))
                .OnExit(() =>
                    SendMessage(new RequestTimeoutCommand(SagaId, _updateInterval)));

            StateMachine.Configure(State.Received)
                .PermitReentry(Trigger.JsonReceived)
                .Permit(Trigger.Parsed, State.Waiting)
                .OnEntry(() => OnReceived(_json))
                .OnExit(OnParsed);

            base.ConfigureStateMachine();
        }

        public void When(AssetTrackingRequested e)
        {
            _url = e.Url;
            _updateInterval = e.UpdateInterval;
        }
        
        public void When(StopTrackingRequested e) { }
        public void When(StartRequested e) { }
        public void When(TimeoutCompleted e) { }
        public void When(JsonRequestFailed e) { }
        public void When(AssetJsonParsed e) { }
        public void When(JsonRequested e) { }
        public void When(JsonReceived e)
        {
            _json = e.Result;
        }
        
        protected virtual void OnReceived (string json) {}
        protected virtual void OnParsed() {}
    }
}