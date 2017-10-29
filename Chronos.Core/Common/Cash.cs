namespace Chronos.Core.Common
{
    public struct Cash
    {
        public string Currency { get; }
        public double Amount { get; }

        public Cash WithAmount(double amount)
        {
            return new Cash(Currency, amount);
        }

        public Cash(string currency, double amount)
        {
            Currency = currency;
            Amount = amount;
        }
    }
}