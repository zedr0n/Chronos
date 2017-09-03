using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;

namespace Chronos.Infrastructure.Commands
{
    public class CommandBus : ICommandBus
    {
        private readonly ICommandRegistry _registry;
        private readonly IDebugLog _debugLog;

        public CommandBus(ICommandRegistry registry, IDebugLog debugLog)
        {
            _registry = registry;
            _debugLog = debugLog;
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