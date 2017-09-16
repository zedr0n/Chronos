using System;
using Chronos.Core.Assets;
using Chronos.Core.Orders.NiceHash.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Orders.NiceHash
{
    public class Order : AggregateBase
    {
        private int _orderNumber;
        private Amount _unitPrice;
        private readonly Status _status;
        private double _maxSpeed;
        
        private class Status
        {
            private readonly Order _order;
            public Amount Spent { get; set; }
            public double Speed { get; set; }
            
            public Status(Order order, Amount spent)
            {
                _order = order;
                Spent = spent;
                Speed = 0.0;
            }
        }
            
        public Order(Guid orderId, int orderNumber, Amount unitPrice)
        {
            When(new NicehashOrderCreated
            {
                OrderId = orderId,
                OrderNumber = orderNumber,
                PriceAsset = unitPrice.AssetId,
                Price = unitPrice.Quantity
            });     
            _status = new Status(this,Amount.Null());
        }

        public void Track(int updateInterval)
        {
            When(new NicehashOrderTrackingRequested
            {
                OrderId = Id,
                OrderNumber = _orderNumber,
                UpdateInterval = updateInterval
            });
        }

        public void UpdateStatus(Amount spent, double speed)
        {
            if (spent.AssetId != _unitPrice.AssetId)
                throw new InvalidOperationException("Spend asset not consistent with price asset");
            
            When(new NicehashOrderUpdated
            {
                OrderId = Id,
                Spent = spent.Quantity,
                Speed = speed
            });
        }

        public void When(NicehashOrderUpdated e)
        {
            _status.Spent = new Amount(_unitPrice.AssetId,e.Spent);
            _status.Speed = e.Speed;
            base.When(e);
        }
        
        public void When(NicehashOrderCreated e)
        {
            Id = e.OrderId;
            _orderNumber = e.OrderNumber;
            _unitPrice = new Amount(e.PriceAsset,e.Price);
            
            base.When(e);
        }
    }
}