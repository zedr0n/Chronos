using System.Diagnostics;
using System.Reflection;
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
            var handler = _registry.GetHandler<T>();
            //Debug.WriteLine(command.GetType().Name + "! -> " + handler?.GetType().Name);
            _debugLog.WriteLine(command.GetType().Name + "! -> " + _registry.GetHandlerName(command));
            handler?.Invoke(command);
        }

        public void Send(ICommand command)
        {
            if (command == null)
                return;

            var handler = _registry.GetHandler(command);
            _debugLog.WriteLine(command.GetType().Name + "! -> " + _registry.GetHandlerName(command));
            handler?.Invoke(command);
        }
    }
}