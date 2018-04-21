using System;
using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Core.Accounts.Commands
{
    public class CreateAccountCommand : CommandBase
    {
        /// <summary>
        /// Account name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Account currency
        /// </summary>
        public string Currency { get; set; }
    }
}