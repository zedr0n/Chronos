using Chronos.Core.Net.Tracking.Urls;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Tracking.Commands
{
    public class TrackCoinHandler : TrackAssetHandler, ICommandHandler<TrackCoinCommand>
    {
        public TrackCoinHandler(IDomainRepository domainRepository, IUrlProvider urlProvider) : base(domainRepository, urlProvider)
        {
        }

        public void Handle(TrackCoinCommand command)
        {
            var tracker = Tracker(command.TargetId); 
            tracker.TrackCoin(command.AssetId,command.Ticker,command.UpdateInterval,Url(command.Ticker));
            Save(tracker);
        }
    }
}