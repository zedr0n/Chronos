using System;
using System.Collections.Generic;
using Chronos.Core.Assets;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions
{
    public class Purchase : AggregateBase, IConsumer<PurchaseCreated>
    {
        private Guid _accountId;
        private Cash _cash;
        private string _payee;

        public Purchase() { }

        public Purchase(Guid id, 
                        Guid accountId,
                        string payee,
                        Cash cash)
            : base(id)
        {
            RaiseEvent(new PurchaseCreated
            {
                SourceId = id,
                AccountId = accountId,
                Payee = payee,
                Currency = cash.Currency,
                Amount = cash.Amount
            });
        }

        public void When(PurchaseCreated e)
        {
            _accountId = e.AccountId;
            _payee = e.Payee;
            _cash = new Cash(e.Currency, e.Amount);
        }
    }
}