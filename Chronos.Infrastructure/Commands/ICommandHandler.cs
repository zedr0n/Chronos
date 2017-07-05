namespace Chronos.Infrastructure.Commands
{
    public interface ICommand { }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}