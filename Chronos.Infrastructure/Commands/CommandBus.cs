using System.Threading.Tasks;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    /// <summary>
    /// Chronos sync/async command bus
    /// </summary>
    public class CommandBus : ICommandBus
    {
        private readonly ICommandRegistry _registry;

        public CommandBus(ICommandRegistry registry)
        {
            _registry = registry;
        }

        /// <summary>
        /// Send <paramref name="command"/> to the registered handlers
        /// </summary>
        /// <param name="command">Command instance</param>
        /// <typeparam name="T">Command type</typeparam>
        public void Send<T>(T command) where T : class,ICommand
        {
            var handler = _registry.Get<T>();
            handler.Handle(command);
        }

        public async Task SendAsync(ICommand command)
        {
            await Task.Run(() => Send(command));
        }

        /// <summary>
        /// Send <paramref name="command"/> to the registered handlers
        /// </summary>
        /// <param name="command"></param>
        public void Send(ICommand command) => Send((dynamic) command);
    }
}