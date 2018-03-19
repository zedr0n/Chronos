using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class WithdrawAssetCommand : CommandBase
    {
        /// <summary>
        /// Asset id
        /// </summary>
        public Guid AssetId { get; set; }
    }
}