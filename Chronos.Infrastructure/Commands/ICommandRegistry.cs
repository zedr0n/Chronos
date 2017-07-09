using System;

namespace Chronos.Infrastructure.Commands
{
    public interface ICommandRegistry
    {
        ICommandHandler<T> GetHandler<T>() where T : ICommand;
    }
}