using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class ChangeAccountCommand : ICommand
    {
        public Guid Guid { get; set; }
        public virtual string Name { get; set; }
        public virtual string Currency { get; set; }
    }
}