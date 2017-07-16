using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public interface ICommandRegistry
    {
        Action<T> GetHandler<T>() where T : class,ICommand;
        Action<ICommand> GetHandler(ICommand command);
        string GetHandlerName(ICommand command);

    }
}