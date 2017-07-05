using System;
using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Core.Account.Commands
{
    public class CreateAccountCommand : ICommand
    {
        public Guid Guid { get; set; }
        public virtual string Name { get; set; }
        public virtual string Currency { get; set; }
        public virtual Instant Date { get; set; }
    }
}