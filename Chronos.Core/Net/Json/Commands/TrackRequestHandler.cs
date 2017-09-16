using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Json.Commands
{
    public class TrackRequestHandler<T> : ICommandHandler<TrackRequestCommand<T>> where T : class
    {
        private readonly IDomainRepository _domainRepository;
    
        public TrackRequestHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }
    
        public void Handle(TrackRequestCommand<T> command)
        {
            var request = _domainRepository.Find<Request<T>>(command.TargetId);        
            request.Track(command.UpdateInterval);
            _domainRepository.Save(request);
        }
    }
    
        
}