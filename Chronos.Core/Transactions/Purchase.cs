using System;
using System.Collections.Generic;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure.Aggregates;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Core.Transactions
{
    public class Purchase : AggregateBase, IConsumer<PurchaseCreated>
    {
        private Guid _accountId;
        private string _payee;
        private string _currency;
        private double _amount;
        private Instant _date;

        public Purchase(Guid id) : base(id)
        {
        }
        public Purchase(Guid id, IEnumerable<IEvent> pastEvents) 
            : base(id,pastEvents) { }

        public Purchase(Guid id, Guid accountId,
            string payee, string ccy, double amount, Instant date) : this(id)
        {
            RaiseEvent(new PurchaseCreated
            {
                AccountId = accountId,
                Payee = payee,
                Ccy = ccy,
                Amount = amount,
                Date = date
            });
        }

        public void When(PurchaseCreated e)
        {
            _accountId = e.AccountId;
            _payee = e.Payee;
            _currency = e.Ccy;
            _amount = e.Amount;
            _date = e.Date;
        }

    }
}