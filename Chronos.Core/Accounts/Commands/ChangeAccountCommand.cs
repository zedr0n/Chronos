using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class ChangeAccountCommand : CommandBase
    {
        public string Name { get; set; }
        public string Currency { get; set; }
    }
}