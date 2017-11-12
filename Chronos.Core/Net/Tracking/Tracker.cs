using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Core.Net.Tracking
{
    public class Tracker : AggregateBase
    {
        public static readonly Guid TrackerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        //private readonly ConcurrentDictionary<Guid,IEvent> _trackedEntities = new ConcurrentDictionary<Guid, IEvent>();    
        private readonly HashSet<Guid> _tracked = new HashSet<Guid>();
        
        public Tracker()
        {
            Id = TrackerId;
        }
        
        protected override void When(IEvent e)
        {
            When((dynamic) e);
        }

        private void When(StartRequested e)
        {
            base.When(e);
        }
        
        public void StartTracking(Guid? assetId)
        {
            if (assetId != null)
            {
                if (_tracked.Contains(assetId.Value))
                {
                    When(new StartRequested(assetId.Value));
                    return;
                }
            }

            foreach (var id in _tracked)
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
        
        private void When(AssetTrackingRequested e)
        {
            if (!_tracked.Add(e.AssetId))
                return;
            
            base.When(e);
        }
    }
}