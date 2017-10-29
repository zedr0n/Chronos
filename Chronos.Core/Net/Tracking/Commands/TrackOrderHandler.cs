using Chronos.Core.Common;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Tracking.Commands
{
    public class TrackOrderHandler : TrackAssetHandler,ICommandHandler<TrackOrderCommand>
    {
        protected override AssetType AssetType => AssetType.Order;
        
        public void Handle(TrackOrderCommand command)
        {
            base.Handle(command,Url());
        }

        public TrackOrderHandler(IDomainRepository domainRepository, IUrlProvider urlProvider) : base(domainRepository, urlProvider)
        {
        }
    }
}
