using System;
using Chronos.Core.Net.Parsing.Commands;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure.Sagas;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class OrderTrackingSaga : AssetTrackingSaga,
        IHandle<OrderTrackingRequested>
    {
        private int _orderNumber;
        private Guid _orderId;
        
        public void When(OrderTrackingRequested e)
        {
            base.When(e);
        }

        protected override void OnTracking(AssetTrackingRequested e)
        {
            _orderNumber = ((OrderTrackingRequested) e).OrderNumber;
            _orderId = e.AssetId;
            base.OnTracking(e);
        }

        protected override void OnReceived(string json)
        {
            var command = new ParseOrderCommand(_orderId, _orderNumber, json);
            SendMessage(command);
            base.OnReceived(json);
        }
    }
}