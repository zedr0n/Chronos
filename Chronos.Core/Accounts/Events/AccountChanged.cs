using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Accounts.Events
{
    /// <summary>
    /// Account details changed event
    /// </summary>
    public class AccountChanged : EventBase
    {
        public AccountChanged(Guid accountId, string name, string currency)
        {
            AccountId = accountId;
            Name = name;
            Currency = currency;
        }

        /// <summary>
        /// Account id
        /// </summary>
        public Guid AccountId { get; }
        /// <summary>
        /// Account name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Account currency
        /// </summary>
        public string Currency { get; }
    }
}