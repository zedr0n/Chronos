using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public class RequestJsonHandler<T> : ICommandHandler<RequestJsonCommand<T>>
        where T : class
    {
        private readonly IJsonConnector _jsonConnector;
        private readonly IEventStoreConnection _connection;
	
        public RequestJsonHandler(IJsonConnector jsonConnector, IEventStoreConnection connection)
        {
            _jsonConnector = jsonConnector;
            _connection = connection;
        }

        public void Handle(RequestJsonCommand<T> command)
        {
            var result = _jsonConnector.Get<T>(command.Url);
            _jsonConnector.Save(command.RequestId,result); 

            var events = new IEvent[]
            {    
                new JsonRequestCompleted
                {
                    RequestId = command.RequestId,
                    RequestorId = command.TargetId
                }
            };
            
            _connection.Writer.AppendToNull(events);
            //command.Handler(obj);
        }
    }

    
}