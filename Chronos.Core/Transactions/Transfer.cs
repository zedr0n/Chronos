using System;
using Chronos.Infrastructure;

namespace Chronos.Core.Transactions
{
    public abstract class Transfer : AggregateBase
    {
        protected TransferDetails TransferDetails { get; set; }
        protected string Description { get; set; }

        protected Transfer() { }

        protected Transfer(Guid id) : base(id) { }
    }
}