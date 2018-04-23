using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Coinbase.Commands
{
    public class PurchaseCoinCommand : CommandBase
    {
        public Guid PurchaseId { get; }
        public Guid CoinId { get; }
        public double Quantity { get; }
        public double CostPerUnit { get; }
        public double Fee { get; }

        public PurchaseCoinCommand(Guid purchaseId, Guid coinId, double quantity, double costPerUnit, double fee)
        {
            PurchaseId = purchaseId;
            CoinId = coinId;
            Quantity = quantity;
            CostPerUnit = costPerUnit;
            Fee = fee;
        }
    }
}