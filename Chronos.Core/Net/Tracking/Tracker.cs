using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Chronos.Core.Common;
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
        
        private void When(Guid id, IEvent e)
        {
            if (!_trackedEntities.TryAdd(id,e))
                return;
            
            When(e);     
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

            When(id,@event);
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
            
            When(id,@event);
        }
    }
}