namespace Chronos.Infrastructure.Commands
{
    public interface ICommandBus
    {
        void Send<T>(T e) where T : ICommand;
    }
}