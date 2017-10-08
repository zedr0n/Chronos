using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public class CommandRecorder<T> : ICommandHandler<T> where T : ICommand
    {
        private readonly ICommandHandler<T> _handler;
        private readonly IEventStoreConnection _connection;
        
        public CommandRecorder(ICommandHandler<T> handler, IEventStoreConnection connection)
        {
            _handler = handler;
            _connection = connection;
        }

        public void Handle(T command)
        {
            _handler.Handle(command);
            _connection.AppendCommand(command);
        }
    }
}