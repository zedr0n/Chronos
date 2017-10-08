using Chronos.Infrastructure.Interfaces;
using Chronos.Persistence.Types;

namespace Chronos.Persistence.Serialization
{
    public interface ICommandSerializer
    {
        Command Serialize(ICommand command);

        ICommand Deserialize(Command command);
    }
}