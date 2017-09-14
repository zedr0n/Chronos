using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public abstract class ParseJsonRequestHandler<T,TCommand> 
        where T : class 
        where TCommand : class,ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;
        private readonly IJsonConnector _jsonConnector;

        public ParseJsonRequestHandler(ICommandHandler<TCommand> handler, IJsonConnector jsonConnector)
        {
            _handler = handler;
            _jsonConnector = jsonConnector;
        }

        public void HandleInternal(ParseJsonRequestCommand<T, TCommand> command, Func<T,TCommand> aggregateCommand)
        {
            var result = _jsonConnector.Find<T>(command.RequestId);
            _handler.Handle(aggregateCommand(result));
        }
    }
}