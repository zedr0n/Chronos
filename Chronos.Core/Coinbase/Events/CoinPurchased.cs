using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Coinbase.Events
{
    public class CoinPurchased : EventBase
    {
        public Guid AccountId { get; }
        public Guid PurchaseId { get; }
        public Guid CoinId { get; }
        public double Quantity { get; }
        public double CostPerUnit { get; } // in GBP
        public double Fee { get; }

        public CoinPurchased(Guid accountId, Guid purchaseId, Guid coinId, double quantity, double costPerUnit, double fee)
        {
            AccountId = accountId;
            PurchaseId = purchaseId;
            CoinId = coinId;
            Quantity = quantity;
            CostPerUnit = costPerUnit;
            Fee = fee;
        }
    }
}