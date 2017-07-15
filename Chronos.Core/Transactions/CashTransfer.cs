using System;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions
{
    public class CashTransfer : Transfer, IConsumer<CashTransferCreated>
    {
        private Cash _cash;

        public CashTransfer() { }

        protected CashTransfer(Guid id) : base(id) { }

        public CashTransfer(Guid id, Guid accountFrom, Guid accountTo, Cash cash)
        {
            RaiseEvent(new CashTransferCreated
            {
                SourceId = id,
                FromAccount = accountFrom,
                ToAccount = accountTo,
                Amount = cash.Amount,
                Currency = cash.Currency
            });
        }

        public void When(CashTransferCreated e)
        {
            TransferDetails = new TransferDetails(e.FromAccount,e.ToAccount);
            _cash = new Cash(e.Currency,e.Amount);
        }
    }
}