using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Orders.NiceHash.Commands
{
    public class TrackOrderHandler : ICommandHandler<TrackOrderCommand>
    {
        private readonly IDomainRepository _domainRepository;
        
        public TrackOrderHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(TrackOrderCommand command)
        {
            var order = _domainRepository.Find<Order>(command.TargetId);
            order.Track(command.UpdateInterval);
            _domainRepository.Save(order);
        }
    }

 
}