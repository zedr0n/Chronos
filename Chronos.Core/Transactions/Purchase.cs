using System;
using System.Collections.Generic;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions
{
    public partial class Purchase : AggregateBase, IConsumer<PurchaseCreated>
    {
        private Guid _accountId;
        private PurchaseInfo _purchaseInfo;

        public Purchase(Guid id, IEnumerable<IEvent> pastEvents) 
            : base(id,pastEvents) { }

        public Purchase(Guid id, Guid accountId,
            PurchaseInfo info) : base(id)
        {
            RaiseEvent(new PurchaseCreated
            {
                SourceId = id,
                AccountId = accountId,
                Payee = info.Payee,
                Ccy = info.Currency,
                Amount = info.Amount,
            });
        }

        public void When(PurchaseCreated e)
        {
            _accountId = e.AccountId;
            _purchaseInfo = new PurchaseInfo
            {
                Payee = e.Payee,
                Amount = e.Amount,
                Currency = e.Ccy,
            };
        }

    }
}