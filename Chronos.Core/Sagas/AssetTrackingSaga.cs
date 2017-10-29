using System;
using Chronos.Core.Common;
using Chronos.Core.Net.Parsing.Commands;
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
        IHandle<AssetTrackingRequested>,
        IHandle<AssetJsonParsed>,
        IHandle<TimeoutCompleted>
    {
        public enum State
        {
            Open,
            Active,
            Paused,
            Received,
            Completed
        }
        public enum Trigger
        {
            TrackingRequested,
            JsonReceived,
            Pause,
            Parsed 
        }
        
        public AssetTrackingSaga()
        {
        }

        private Guid _assetId;
        private string _url;
        private Duration _updateInterval;
        private AssetType _assetType;

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
                .Permit(Trigger.TrackingRequested, State.Active);

            StateMachine.Configure(State.Active)
                .Permit(Trigger.Pause, State.Paused)
                .Permit(Trigger.JsonReceived, State.Received)
                .OnEntryFrom(_trackingTrigger, OnTracking)
                .OnEntry(RequestTimeout);

            StateMachine.Configure(State.Received)
                .OnEntryFrom(_jsonReceivedTrigger, OnReceived)
                .Permit(Trigger.Parsed, State.Active);
            
            base.ConfigureStateMachine();
        }
        
        public void When(AssetTrackingRequested e)
        {
            StateMachine.Fire(_trackingTrigger,e);    
            base.When(e);
        }
        
        private void OnTracking(AssetTrackingRequested e)
        {
            _url = e.Url;
            _assetId = e.AssetId;
            _assetType = e.AssetType;
            
            _updateInterval = e.UpdateInterval;
        }

        private void RequestTimeout()
        {
            SendMessage(new RequestTimeoutCommand(_assetId, _updateInterval));
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
            base.When(e);
        }
        
        private void OnReceived(string json)
        {
            RequestParsingCommand command = null;
            switch (_assetType)
            {
                case AssetType.Order:
                    command = new ParseOrderCommand(_assetId,json);
                    break;
                case AssetType.Coin:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SendMessage(command);
        }

        public void When(AssetJsonParsed e)
        {
            StateMachine.Fire(Trigger.Parsed);
            base.When(e);
        }
    }
}