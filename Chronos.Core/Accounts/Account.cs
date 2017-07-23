using System;
using System.Collections.Generic;
using Chronos.Core.Accounts.Events;
using Chronos.Core.Transactions;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using NodaTime;
using Chronos.Core.Assets;

namespace Chronos.Core.Accounts
{
    public class Account : AggregateBase,
        IConsumer<AccountCreated>,
        IConsumer<AccountChanged>,
        IConsumer<CashDeposited>,
        IConsumer<AssetDeposited>,
        IConsumer<CashWithdrawn>
    {
        private string _name;
        private string _currency;
        private Instant _createdAt;

        private Cash _cash;
        private readonly HashSet<Guid> _assets = new HashSet<Guid>();

        public Account() { }
        public Account(Guid id, string name, string ccy)
            : base(id)
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
        /// @<see cref="Account"/> : <see cref="CashDeposited"/>! -> <see cref="When(CashDeposited)"/>
        /// </summary>
        /// <param name="amount">Debit amount</param>
        public void Debit(double amount)
        {
            RaiseEvent( new CashDeposited
            {
                SourceId = Id,
                Amount = amount
            });
        }
        public void Credit(double amount)
        {
            RaiseEvent(new CashWithdrawn
            {
                SourceId = Id,
                Amount = amount
            });
        }
        public void DepositAsset( Guid assetId )
        {
            RaiseEvent( new AssetDeposited
            {
                SourceId = Id,
                AssetId = assetId
            });
        }

        public void When(AccountCreated e)
        {
            _name = e.Name;
            _currency = e.Currency;
            _createdAt = e.Timestamp;
            _cash = new Cash(_currency,0);
        }
        public void When(AccountChanged e)
        {
            _name = e.Name;
            _currency = e.Currency;
        }
        public void When(CashDeposited e)
        {
            _cash = _cash.WithAmount(_cash.Amount + e.Amount);
        }
        public void When(AssetDeposited e)
        {
            _assets.Add(e.AssetId);
        }
        public void When(CashWithdrawn e)
        {
            _cash = _cash.WithAmount(_cash.Amount - e.Amount);
        }
    }
}