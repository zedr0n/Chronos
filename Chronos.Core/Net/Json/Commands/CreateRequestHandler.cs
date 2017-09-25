using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Json.Commands
{
    public class CreateRequestHandler<T> : ICommandHandler<CreateRequestCommand<T>> where T : class
    {
        private readonly IDomainRepository _domainRepository;

        public CreateRequestHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }
    
        public void Handle(CreateRequestCommand<T> command)
        {
            //if(_domainRepository.Exists<Request<T>>(command.TargetId))
            //    throw new InvalidOperationException("Request already exists");
            
    		var request = new Request<T>(command.TargetId,command.Url);  
            _domainRepository.Save(request);
        }
    }
}