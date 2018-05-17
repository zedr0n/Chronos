using System;
using Chronos.Core.Net.Parsing.Commands;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Sagas;
using Stateless;

namespace Chronos.Core.Sagas
{
    public class OrderTrackingSaga : AssetTrackingSaga,
        IHandle<OrderTrackingRequested>
    {
        private int _orderNumber;
        private Guid _orderId;

        public OrderTrackingSaga()
        {
            Register<OrderTrackingRequested>(Trigger.TrackingRequested);
        }

        protected override void Handle(IEvent e) => When((dynamic) e);

        public void When(OrderTrackingRequested e)
        {
            _orderNumber = e.OrderNumber;
            _orderId = e.AssetId;
            base.When(e);    
        }

        protected override void OnReceived(string json)
        {
            var command = new ParseOrderCommand(_orderId, _orderNumber, json);
            SendMessage(command);
            base.OnReceived(json);
        }
    }
}