using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Coinbase.Commands
{
    public class CreateCoinbaseAccountCommand : CommandBase
    {
        public string Email { get; }

        public CreateCoinbaseAccountCommand(string email)
        {
            Email = email;
        }
    }
}