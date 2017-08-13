using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Interfaces;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class CommandRegistry : ICommandRegistry
    {
        private readonly Dictionary<Type, Handler> _registry = new Dictionary<Type, Handler>();
        private readonly Dictionary<Type, ICommandHandler> _handlers = new Dictionary<Type, ICommandHandler>();

        private struct Handler
        {
            private readonly Type _type;
            internal string Name => _type.Name;
            internal Action<ICommand> Action { get; }

            internal Handler(ICommandHandler handler, Action<ICommand> action)
            {
                _type = handler.GetType();
                Action = action;
            }
        }

        public CommandRegistry(Container container)
        {
            foreach(var handler in container.GetCurrentRegistrations()
                .Where(x => x.ServiceType.IsClosedTypeOf(typeof(ICommandHandler<>)))
                .Select(x => new
                {
                    Instance = x.GetInstance() as ICommandHandler,
                    CommandType = x.ServiceType.GetClosedTypeOf(typeof(ICommandHandler<>)).GetGenericArguments().Single()
                } ))
            {
                var methodInfo = handler.Instance.GetType().GetMethod("Handle");
                _handlers[handler.CommandType] = handler.Instance;

                _registry[handler.CommandType] = new Handler(handler.Instance,
                    c => methodInfo.Invoke(handler.Instance,new object[] { c })); //c => methodInfo.Invoke(handler,new object[] { c });
            }
        }
        
        public ICommandHandler<T> Get<T>() where T : class, ICommand
        {
            return _handlers[typeof(T)] as ICommandHandler<T>;
        }
        
        public Action<T> GetHandler<T>() where T : class,ICommand
        {
            return _registry[typeof(T)].Action;
        }

        public string GetHandlerName(ICommand command)
        {
            return _registry[command.GetType()].Name;
        }

        public Action<ICommand> GetHandler(ICommand command)
        {
            var type = command.GetType();
            if (!_registry.ContainsKey(type))
                return null;

            return _registry[type].Action;
        }
    }
}