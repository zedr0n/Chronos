namespace Chronos.Infrastructure.Commands
{
    public class RequestJSONHandler<T> : ICommandHandler<RequestJSONCommand<T>>
        where T : new()
    {
        private readonly IEventStoreConnection _connection;
        private readonly IJSONConnector _jsonConnector;
	
        public RequestJSONHandler(IEventStoreConnection connection, IJSONConnector jsonConnector)
        {
            _connection = connection;
            _jsonConnector = jsonConnector;
        }

        public void Handle(RequestJSONCommand<T> command)
        {
            var obj = _jsonConnector.Get<T>(command.Url);
            command.Handler(obj);
        }
    }

    
}