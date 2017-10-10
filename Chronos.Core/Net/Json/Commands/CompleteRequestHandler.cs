using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Json.Commands
{
    public class CompleteRequestHandler<T> : ICommandHandler<CompleteRequestCommand> where T : class
    {
        private readonly IDomainRepository _domainRepository;

        public CompleteRequestHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(CompleteRequestCommand command)
        {
            var request = _domainRepository.Find<Request<T>>(command.TargetId); 
            request.Complete();
            _domainRepository.Save(request);
        }
    }
}