using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Aggregates;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Account
{
    public class Aggregate : AggregateBase,
        IConsumer<Created>,
        IConsumer<Changed>,
        IConsumer<AmountDebited>
    {
        private string Name { get; set; }
        private string Currency { get; set; }
        private double Balance { get; set; }

        private Aggregate(Guid id) : base(id)
        {
        }

        public Aggregate(Guid id, IEnumerable<IEvent> pastEvents)
            : base(id,pastEvents) { }

        public Aggregate(Guid id, string name, string ccy)
            : this(id)
        {
            RaiseEvent(new Created
            {
                SourceId = id,
                Name = name,
                Currency = ccy
            });
        }

        /// <summary>
        /// @<see cref="Aggregate"/> : <see cref="Changed"/>! -> <see cref="When(Changed)"/>
        /// </summary>
        /// <param name="name">Account name</param>
        /// <param name="currency">Account currency</param>
        public void ChangeDetails(string name, string currency)
        {
            RaiseEvent(new Changed
            {
                SourceId = Id,
                Name = name,
                Currency = currency
            });
        }

        /// <summary>
        /// @<see cref="Aggregate"/> : <see cref="AmountDebited"/>! -> <see cref="When(AmountDebited)"/>
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

        public void When(Created e)
        {
            Name = e.Name;
            Currency = e.Currency;
        }

        public void When(Changed e)
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