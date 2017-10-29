using Chronos.Core.Common;
using Chronos.Core.Common.Commands;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Net.Tracking.Commands
{
    public abstract class TrackAssetHandler 
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IUrlProvider _urlProvider;

        protected string Url(params object[] args) => _urlProvider.Get(args);

        protected TrackAssetHandler(IDomainRepository domainRepository, IUrlProvider urlProvider)
        {
            _domainRepository = domainRepository;
            _urlProvider = urlProvider;
        }

        public void Handle(TrackAssetCommand command,string url)
        {
            var tracker = _domainRepository.Find<Tracker>(command.TargetId) ?? new Tracker();
            tracker.TrackAsset(command.AssetId,AssetType,command.UpdateInterval,url);
            _domainRepository.Save(tracker);
        }

        protected abstract AssetType AssetType { get; }
    }
}