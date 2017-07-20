using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections;
using NodaTime;

namespace Chronos.Core.Accounts.Projections
{
    public class AccountInfo : ProjectionBase<Guid>, IReadModel<Guid>
    {
        public string Name { get; set; }
        public string Currency { get; set; }
        public double Balance { get; set; }
        public Instant CreatedAt { get; set; }
    }
}