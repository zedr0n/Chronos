namespace Chronos.Infrastructure.Commands
{
    public class CommandBus : ICommandBus
    {
        private readonly ICommandRegistry _registry;

        public CommandBus(ICommandRegistry registry)
        {
            _registry = registry;
        }

        public void Send<T>(T command) where T : ICommand
        {
            var handler = _registry.GetHandler<T>();
            handler?.Handle(command);
        }
    }
}