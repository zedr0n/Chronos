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

        private void When(Guid id, IEvent e)
        {
            if (!_trackedEntities.TryAdd(id,e))
                return;
            
            When(e);     
        }
        
        public void TrackAsset(Guid id, AssetType assetType,Duration updateInterval,string url)
        {
            var @event = new AssetTrackingRequested
            {
                AssetId = id,
                UpdateInterval = updateInterval,
                Url = url,
                AssetType = assetType
            }; 
            
            When(id,@event);
        }

        public void TrackOrder(Guid id, int orderNumber, Duration updateInterval, string url)
        {
            var @event = new OrderTrackingRequested
            {
                AssetId = id,
                AssetType = AssetType.Order,
                UpdateInterval = updateInterval,
                Url = url,
                OrderNumber = orderNumber
            };

            When(id,@event);
        }
    }
}