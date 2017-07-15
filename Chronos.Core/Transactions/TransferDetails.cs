using System;

namespace Chronos.Core.Transactions
{
    public class TransferDetails
    {
        public Guid AccountFrom { get; }
        public Guid AccountTo { get; }

        public TransferDetails(Guid accountFrom, Guid accountTo)
        {
            AccountFrom = accountFrom;
            AccountTo = accountTo;
        }
    }
}