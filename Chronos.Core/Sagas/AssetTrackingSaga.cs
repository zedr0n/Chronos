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
    public class AssetTrackingSaga : StatelessSaga<AssetTrackingSaga.State,AssetTrackingSaga.Trigger>
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

        public AssetTrackingSaga()
        {
            Register<StopTrackingRequested>(Trigger.Stop);
            Register<StartRequested>(Trigger.Start);
            Register<JsonRequested>(Trigger.JsonRequested);
            Register<JsonReceived>(Trigger.JsonReceived, When);
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
                .OnEntry(() =>
                    SendMessage(new RequestJsonCommand(_url, SagaId)))
                .Permit(Trigger.JsonRequested, State.Waiting)
                .Ignore(Trigger.Parsed)
                .OnExit(() =>
                    SendMessage(new RequestTimeoutCommand(SagaId, _updateInterval)));

            StateMachine.Configure(State.Received)
                .PermitReentry(Trigger.JsonReceived)
                .Permit(Trigger.Parsed, State.Waiting)
                .Permit(Trigger.Start,State.Requesting)
                .Permit(Trigger.Stop,State.Open)
                .OnEntry(() => OnReceived(_json))
                .OnExit(OnParsed);

            base.ConfigureStateMachine();
        }

        protected void When(AssetTrackingRequested e)
        {
            _url = e.Url;
            _updateInterval = e.UpdateInterval;
        }

        private void When(JsonReceived e) => _json = e.Result;
        
        protected virtual void OnReceived (string json) {}
        protected virtual void OnParsed() {}
    }
}