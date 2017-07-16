using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public interface ICommandHandler
    {    }
    public interface ICommandHandler<in TCommand>  : ICommandHandler where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}