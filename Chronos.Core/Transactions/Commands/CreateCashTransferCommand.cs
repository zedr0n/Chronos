namespace Chronos.Core.Transactions.Commands
{
    public class CreateCashTransferCommand : CreateTransferCommand
    {
        public string Currency { get; set; }
        public double Amount { get; set; }
    }
}