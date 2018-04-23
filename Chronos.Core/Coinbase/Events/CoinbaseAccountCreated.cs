using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Coinbase.Events
{
    public class CoinbaseAccountCreated : EventBase
    {
        public Guid AccountId { get; }
        public string Email { get; }

        public CoinbaseAccountCreated(Guid accountId, string email)
        {
            AccountId = accountId;
            Email = email;
        }
    }
}