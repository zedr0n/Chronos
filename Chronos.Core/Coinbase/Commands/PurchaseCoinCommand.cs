using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Coinbase.Commands
{
    public class PurchaseCoinCommand : CommandBase
    {
        public Guid PurchaseId { get; }
        public string Coin { get; }
        public double Quantity { get; }
        public double CostPerUnit { get; }
        public double Fee { get; }

        public PurchaseCoinCommand(Guid purchaseId, string coin, double quantity, double costPerUnit, double fee)
        {
            PurchaseId = purchaseId;
            Coin = coin;
            Quantity = quantity;
            CostPerUnit = costPerUnit;
            Fee = fee;
        }
    }
}