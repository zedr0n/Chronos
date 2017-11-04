using System;
using System.Collections.Concurrent;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Core.Net.Tracking
{
    public class Tracker : AggregateBase
    {
        public static readonly Guid TrackerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private readonly ConcurrentDictionary<Guid,IEvent> _trackedEntities = new ConcurrentDictionary<Guid, IEvent>();

        public Tracker()
        {
            Id = TrackerId;
        }

        protected override void When(IEvent e)
        {
            if (e is AssetTrackingRequested o)
                if (!_trackedEntities.TryAdd(o.AssetId, e))
                    return;
            
            base.When(e);
        }
        
        /*private void When(Guid id, IEvent e)
        {
            if (!_trackedEntities.TryAdd(id,e))
                return;
            
            When(e);     
        }*/

        public void StartTracking(Guid? assetId)
        {
            if (assetId != null)
            {
                if (_trackedEntities.ContainsKey(assetId.Value))
                {
                    When(new StartRequested(assetId.Value));
                    return;
                }
            }

            foreach (var id in _trackedEntities.Keys)
                When(new StartRequested(id));
        }

        public void TrackOrder(Guid id, int orderNumber, Duration updateInterval, string url)
        {
            var @event = new OrderTrackingRequested
            {
                AssetId = id,
                UpdateInterval = updateInterval,
                Url = url,
                OrderNumber = orderNumber
            };

            When(@event);
        }

        public void TrackCoin(Guid id, string ticker, Duration updateInterval, string url)
        {
            var @event = new CoinTrackingRequested
            {
                AssetId = id,
                UpdateInterval = updateInterval,
                Url = url,
                Ticker = ticker
            };
            
            When(@event);
        }
    }
}