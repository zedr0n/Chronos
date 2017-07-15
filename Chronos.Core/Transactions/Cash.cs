using System;

namespace Chronos.Core.Transactions
{
    public class Cash
    {
        public string Currency { get; private set; }
        public double Amount { get; private set; }

        private Cash() { }

        public Cash WithAmount(double amount)
        {
            return new Cash
            {
                Currency = Currency,
                Amount = amount
            };
        }

        public Cash(string currency, double amount)
        {
            Currency = currency;
            Amount = amount;
        }
    }
}