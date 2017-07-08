using System;
using System.Collections.Generic;
using Chronos.Core.Accounts.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Core.Accounts
{
    public class Account : AggregateBase,
        IConsumer<AccountCreated>,
        IConsumer<AccountChanged>,
        IConsumer<AmountDebited>
    {
        private string _name;
        private string _currency;
        private double _balance;
        private Instant _createdAt;

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
        /// @<see cref="Account"/> : <see cref="AccountChanged"/>! -> <see cref="When(Chronos.Core.Accounts.Events.AccountChanged)"/>
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
        /// @<see cref="Account"/> : <see cref="AmountDebited"/>! -> <see cref="When(Chronos.Core.Accounts.Events.AmountDebited)"/>
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
            _name = e.Name;
            _currency = e.Currency;
            _balance = 0;
            _createdAt = e.Timestamp;
        }

        public void When(AccountChanged e)
        {
            _name = e.Name;
            _currency = e.Currency;
        }

        public void When(AmountDebited e)
        {
            _balance += e.Amount;
        }

    }
}