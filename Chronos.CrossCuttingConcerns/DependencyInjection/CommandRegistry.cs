using System;
using System.Collections.Generic;
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

        private struct Handler
        {
            private Type _type;
            public string Name => _type.Name;
            public Action<ICommand> Action { get; private set; } 

            public static Handler Create(ICommandHandler handler, Action<ICommand> action) 
            {
                return new Handler
                {
                    _type = handler.GetType(),
                    Action = action
                };
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

                _registry[handler.CommandType] = Handler.Create(handler.Instance,
                    c => methodInfo.Invoke(handler.Instance,new object[] { c })); //c => methodInfo.Invoke(handler,new object[] { c });
            }
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