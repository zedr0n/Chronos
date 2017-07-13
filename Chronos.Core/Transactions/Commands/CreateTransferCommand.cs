using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Transactions.Commands
{
    public abstract class CreateTransferCommand : CommandBase
    {
        public Guid FromAccount { get; set; }
        public Guid ToAccount { get; set; }
    }
}