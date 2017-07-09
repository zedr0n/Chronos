using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Commands;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class CommandRegistry : ICommandRegistry
    {
        private readonly Container _container;
        private readonly Dictionary<Type, ICommandHandler> _registry = new Dictionary<Type, ICommandHandler>();

        public CommandRegistry(Container container)
        {
            _container = container;
        }

        public ICommandHandler<T> GetHandler<T>() where T : ICommand
        {
            if (!_registry.ContainsKey(typeof(T)))
                _registry[typeof(T)] = _container.GetInstance<ICommandHandler<T>>();

            return _registry[typeof(T)] as ICommandHandler<T>;
        }
    }
}