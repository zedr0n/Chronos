using System.Threading.Tasks;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public class CommandBus : ICommandBus
    {
        private readonly ICommandRegistry _registry;

        public CommandBus(ICommandRegistry registry)
        {
            _registry = registry;
        }

        public void Send<T>(T command) where T : class,ICommand
        {
            var handler = _registry.Get<T>();
            handler.Handle(command);
        }

        public async Task SendAsync(ICommand command)
        {
            await Task.Run(() => Send(command));
        }

        public void Send(ICommand command) => Send((dynamic) command);
    }
}