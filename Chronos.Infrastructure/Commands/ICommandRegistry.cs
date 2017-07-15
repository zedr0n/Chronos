using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public interface ICommandRegistry
    {
        ICommandHandler<T> GetHandler<T>() where T : ICommand;
    }
}