using System;
using Chronos.Core.Assets;
using Chronos.Core.Common;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions
{
    public class CashTransfer : Transfer, IConsumer<CashTransferCreated>
    {
        private Cash _cash;

        public CashTransfer() { }

        public CashTransfer(Guid id, Guid accountFrom, Guid accountTo, Cash cash)
        {
            When(new CashTransferCreated
            {
                TransferId = id,
                FromAccount = accountFrom,
                ToAccount = accountTo,
                Amount = cash.Amount,
                Currency = cash.Currency
            });
        }

        public void When(CashTransferCreated e)
        {
            Id = e.TransferId;
            TransferDetails = new TransferDetails(e.FromAccount,e.ToAccount);
            _cash = new Cash(e.Currency,e.Amount);
            base.When(e);
        }
    }
}