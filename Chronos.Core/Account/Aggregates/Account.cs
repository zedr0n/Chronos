using System;
using System.Collections.Generic;
using Chronos.Core.Account.Events;
using Chronos.Infrastructure.Aggregates;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Account.Aggregates
{
    public class Account : AggregateBase,
        IConsumer<AccountCreated>,
        IConsumer<AccountChanged>,
        IConsumer<AmountDebited>
    {
        private string Name { get; set; }
        private string Currency { get; set; }
        private double Balance { get; set; }

        private Account(Guid id) : base(id)
        {
        }

        public Account(Guid id, IEnumerable<IEvent> pastEvents)
            : base(id,pastEvents) { }

        public Account(Guid id, string name, string ccy)
            : this(id)
        {
            RaiseEvent(new AccountCreated
            {
                SourceId = id,
                Name = name,
                Currency = ccy
            });
        }

        /// <summary>
        /// @<see cref="Account"/> : <see cref="AccountChanged"/>! -> <see cref="When(Chronos.Core.Account.Events.AccountChanged)"/>
        /// </summary>
        /// <param name="name">Account name</param>
        /// <param name="currency">Account currency</param>
        public void ChangeDetails(string name, string currency)
        {
            RaiseEvent(new AccountChanged
            {
                SourceId = Id,
                Name = name,
                Currency = currency
            });
        }

        /// <summary>
        /// @<see cref="Account"/> : <see cref="AmountDebited"/>! -> <see cref="When(Chronos.Core.Account.Events.AmountDebited)"/>
        /// </summary>
        /// <param name="amount">Debit amount</param>
        public void Debit(double amount)
        {
            RaiseEvent( new AmountDebited
            {
                SourceId = Id,
                Amount = amount
            });
        }

        public void When(AccountCreated e)
        {
            Name = e.Name;
            Currency = e.Currency;
        }

        public void When(AccountChanged e)
        {
            Name = e.Name;
            Currency = e.Currency;
        }

        public void When(AmountDebited e)
        {
            Balance += e.Amount;
        }

    }
}