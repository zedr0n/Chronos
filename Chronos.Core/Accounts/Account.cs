﻿using System;
using System.Collections.Generic;
using Chronos.Core.Accounts.Events;
using Chronos.Infrastructure;
using NodaTime;
using Chronos.Core.Common;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Core.Accounts
{
    /// <summary>
    /// Account aggregate root
    /// </summary>
    public class Account : AggregateBase
    {
        private string _name;
        private string _currency;
        private Instant _createdAt;

        private Cash _cash;
        private readonly HashSet<Guid> _assets = new HashSet<Guid>();
        private readonly Dictionary<Guid, Amount> _positions = new Dictionary<Guid, Amount>();
        
        public Account() { }
        public Account(Guid id, string name, string ccy)
        {
            When(new AccountCreated
            {
                AccountId = id,
                Name = name,
                Currency = ccy
            });

        }

        /// <summary>
        /// Change account <paramref name="name"/> and <paramref name="currency"/>
        /// </summary>
        /// <param name="name">Account name</param>
        /// <param name="currency">Account currency</param>
        public void ChangeDetails(string name, string currency)
        {
            When(new AccountChanged(Id,name,currency));
        }

        /// <summary>
        /// Debit the account with cash <paramref name="amount"/>
        /// </summary>
        /// <param name="amount">Debit amount</param>
        public void Debit(double amount)
        {
            When( new CashDeposited
            {
                AccountId = Id,
                Amount = amount
            });
        }
        
        /// <summary>
        /// Credit the account with cash <paramref name="amount"/>
        /// </summary>
        /// <param name="amount">Withdrawal amount</param>
        public void Credit(double amount)
        {
            When(new CashWithdrawn
            {
                AccountId = Id,
                Amount = amount
            });
        }

        /// <summary>
        /// Deposit asset to the account
        /// </summary>
        /// <param name="assetId">Asset id</param>
        public void DepositAsset(Guid assetId)
        {
            When( new AssetDeposited
                {
                    AccountId = Id,
                    AssetId = assetId
                });
        }

        /// <summary>
        /// Withdraw asset from account
        /// </summary>
        /// <param name="assetId">Asset id</param>
        public void WithdrawAsset(Guid assetId)
        {
            When( new AssetWithdrawn
                {
                    AccountId = Id,
                    AssetId = assetId
                });
        }

        protected override void When(IEvent e)
        {
            When((dynamic) e);
        }

        private void When(AccountCreated e)
        {
            Id = e.AccountId;
            _name = e.Name;
            _currency = e.Currency;
            _createdAt = e.Timestamp;
            _cash = new Cash(_currency,0);
            base.When(e);
        }

        private void When(AccountChanged e)
        {
            _name = e.Name;
            _currency = e.Currency;
            base.When(e);
        }
        private void When(CashDeposited e)
        {
            _cash = _cash.WithAmount(_cash.Amount + e.Amount);
            base.When(e);
        }
        private void When(AssetDeposited e)
        {
            if (_assets.Add(e.AssetId))
                _positions[e.AssetId] = new Amount(e.AssetId, e.Amount);
            else
                _positions[e.AssetId].Add(e.Amount);
            base.When(e);
        }
        private void When(CashWithdrawn e)
        {
            _cash = _cash.WithAmount(_cash.Amount - e.Amount);
            base.When(e);
        }
    }
}