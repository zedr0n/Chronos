using System;
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

        protected Tracker Tracker(Guid id)
        {
            return _domainRepository.Find<Tracker>(id) ?? new Tracker(); 
        }

        protected void Save(Tracker tracker)
        {
            _domainRepository.Save(tracker); 
        }
    }
}