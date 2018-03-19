using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    /// <summary>
    /// Command to change account details
    /// </summary>
    public class ChangeAccountCommand : CommandBase
    {
        /// <summary>
        /// Account name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Account domestic currency
        /// </summary>
        public string Currency { get; set; }
    }
}