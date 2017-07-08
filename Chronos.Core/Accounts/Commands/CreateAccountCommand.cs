using System;
using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Core.Accounts.Commands
{
    public class CreateAccountCommand : CommandBase
    {
        public string Name { get; set; }
        public string Currency { get; set; }
    }
}