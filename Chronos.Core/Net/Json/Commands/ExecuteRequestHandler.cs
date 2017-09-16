using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Json.Commands
{
    public class ExecuteRequestHandler<T> : ICommandHandler<ExecuteRequestCommand<T>> where T : class
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IJsonConnector _connector;
    
        public ExecuteRequestHandler(IDomainRepository domainRepository, IJsonConnector connector)
        {
            _domainRepository = domainRepository;
            _connector = connector;
        }
    
        public void Handle(ExecuteRequestCommand<T> command)
        {
            var request = _domainRepository.Find<Request<T>>(command.TargetId);	   
            request.Execute(_connector);
            _domainRepository.Save(request);
        }
    }
    
    
}