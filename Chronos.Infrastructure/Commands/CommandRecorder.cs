using System;
using Chronos.Infrastructure.Exceptions;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;

namespace Chronos.Infrastructure.Commands
{
    public class CommandRecorder<T> : ICommandHandler<T> where T : ICommand
    {
        private readonly ICommandHandler<T> _handler;
        private readonly IEventStoreConnection _connection;
        private readonly IDebugLog _debugLog;
        
        public CommandRecorder(ICommandHandler<T> handler, IEventStoreConnection connection, IDebugLog debugLog)
        {
            _handler = handler;
            _connection = connection;
            _debugLog = debugLog;
        }

        public void Handle(T command)
        {
            _debugLog.WriteLine("Entering handler of " + command.GetType().Name);
            try
            {
                _handler.Handle(command);
            }
            catch (Exception e)
            {
                _debugLog.WriteLine(e.Message);
                //Console.WriteLine(e);
                // retry the command in case of concurrency exception
                if(e is ConcurrencyException)
                    _handler.Handle(command);
                throw;
            }


            _connection.AppendCommand(command);
        }
    }
}