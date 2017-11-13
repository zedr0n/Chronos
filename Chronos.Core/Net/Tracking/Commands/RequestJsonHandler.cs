using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<string, IDisposable> _subscriptions
            = new ConcurrentDictionary<string, IDisposable>();
        
        public RequestJsonHandler(IJsonConnector jsonConnector, IEventBus eventBus)
        {
            _jsonConnector = jsonConnector;
            _eventBus = eventBus;
        }

        private void Complete(string url)
        {
            _subscriptions.TryRemove(url, out var subscription);
            subscription?.Dispose();
        }
        
        private void OnCompleted(string url, string json,Guid requestorId)
        {
            Complete(url);   
            
            _eventBus.Alert(new JsonReceived(url,json,requestorId));
        }
        
        private void OnError(Exception e,string url,Guid requestorId)
        {
            Complete(url);
            _eventBus.Alert(new JsonRequestFailed(url, requestorId));
        }
        
        public void Handle(RequestJsonCommand command)
        {
            _subscriptions.GetOrAdd(command.Url,
                url =>
                {
                    var request = _jsonConnector.GetRequest(url);
                    return request.Subscribe(x => x.Value.Subscribe(
                        s => OnCompleted(url, s, command.RequestorId),
                        e => OnError(e, url, command.RequestorId)));
                });
            
            _jsonConnector.SubmitRequest(command.Url);
            
            _eventBus.Alert(new JsonRequested(command.Url,command.RequestorId));
        }
    }
}