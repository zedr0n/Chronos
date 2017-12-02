using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class CreateBagCommand : CommandBase
    {
        public CreateBagCommand(string name)
        {
            Name = name;
        }

        public string Name { get; }    
    }
}