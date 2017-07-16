using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public interface ICommandBus
    {
        void Send<T>(T e) where T : class,ICommand;
        void Send(ICommand command);
    }
}