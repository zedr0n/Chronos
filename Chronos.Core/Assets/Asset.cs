using System;
using Chronos.Infrastructure;

namespace Chronos.Core.Assets
{
    public class Asset : AggregateBase
    {
        protected Asset() { }
        protected Asset(Guid id) : base(id) { }
    }
}