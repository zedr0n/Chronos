using System;
using Chronos.Core.Net.Json.Projections;
using Chronos.Core.Net.Json.Queries;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Net.Json.Commands
{
    public class ExecuteRequestHandler<T> : ICommandHandler<ExecuteRequestCommand<T>> where T : class
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IJsonConnector _connector;
        private readonly IQueryHandler<RequestInfoQuery<T>, RequestInfo<T>> _handler;

        public ExecuteRequestHandler(IDomainRepository domainRepository, IJsonConnector connector, IQueryHandler<RequestInfoQuery<T>, RequestInfo<T>> handler)
        {
            _domainRepository = domainRepository;
            _connector = connector;
            _handler = handler;
        }
    
        public void Handle(ExecuteRequestCommand<T> command)
        {
            var requestInfo = _handler.Handle(new RequestInfoQuery<T>
            {
                RequestId = command.TargetId
            });
            
            if(requestInfo == null)
                throw new InvalidOperationException("Request not found");
            if (requestInfo.Url == null)
            {
                var request = _domainRepository.Find<Request<T>>(command.TargetId);	   
                request.Fail();
                _domainRepository.Save(request);
                return;
            }
                //throw new InvalidOperationException("Request url not set");

            _connector.AsObservable<T>(command.TargetId, requestInfo.Url)
                .Subscribe(x =>
                {
                    _connector.Save(command.TargetId,x);
                    var request = _domainRepository.Find<Request<T>>(command.TargetId);	   
                    request.Complete();
                    _domainRepository.Save(request);
                });
        }
    }
}