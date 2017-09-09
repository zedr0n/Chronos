using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class CreateCoinCommand : CommandBase
    {
        public string Name { get; set; }
        public string Ticker { get; set; }
    }
}