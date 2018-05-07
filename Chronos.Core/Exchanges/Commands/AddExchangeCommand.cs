using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Exchanges.Commands
{
    public class AddExchangeCommand : CommandBase
    {
        public AddExchangeCommand(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}