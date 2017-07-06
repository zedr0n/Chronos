using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class DebitAmountCommand : ICommand
    {
        public Guid Guid { get; set; }
        public double Amount { get; set; }
    }
}