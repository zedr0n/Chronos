using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Tracking.Commands
{
    public class TrackOrderHandler : TrackAssetHandler,ICommandHandler<TrackOrderCommand>
    {
        public void Handle(TrackOrderCommand command)
        {
            var tracker = Tracker(command.TargetId); 
            tracker.TrackOrder(command.AssetId,command.OrderNumber,command.UpdateInterval,Url());
            Save(tracker);
        }

        public TrackOrderHandler(IDomainRepository domainRepository, IUrlProvider urlProvider) : base(domainRepository, urlProvider)
        {
        }
    }
}
