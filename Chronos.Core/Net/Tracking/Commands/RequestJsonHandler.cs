using System;
using Chronos.Core.Common.Commands;
using Chronos.Core.Common.Events;
using Chronos.Core.Net.Tracking.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Tracking.Commands
{
    public class RequestJsonHandler : ICommandHandler<RequestJsonCommand>
    {
        private readonly IJsonConnector _jsonConnector;
        private readonly IEventBus _eventBus;

        public RequestJsonHandler(IJsonConnector jsonConnector, IEventBus eventBus)
        {
            _jsonConnector = jsonConnector;
            _eventBus = eventBus;
        }

        private void OnCompleted(string url, string json,Guid requestorId)
        {
            _eventBus.Alert(new JsonReceived(url,json,requestorId));
        }
        
        private void OnError(Exception e,string url,Guid requestorId)
        {
            _eventBus.Alert(new JsonRequestFailed(url, requestorId));
        }
        
        public void Handle(RequestJsonCommand command)
        {
            _jsonConnector.Request(command.Url)
                .Subscribe(s => OnCompleted(command.Url,s,command.RequestorId), 
                    e => OnError(e,command.Url,command.RequestorId)); 
            
            _eventBus.Alert(new JsonRequested(command.Url,command.RequestorId));
        }
    }
}