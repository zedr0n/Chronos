using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class DepositAssetCommand : CommandBase
    {
        public Guid AssetId { get; set; }
    }
}