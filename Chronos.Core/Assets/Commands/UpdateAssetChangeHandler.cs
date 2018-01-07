using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class UpdateAssetChangeHandler : ICommandHandler<UpdateAssetChangeCommand>
    {
        private readonly IDomainRepository _domainRepository;

        public UpdateAssetChangeHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(UpdateAssetChangeCommand command)
        {
            var asset = _domainRepository.Find<IAsset>(command.TargetId);
            if(asset == null)
                throw new InvalidOperationException("Asset does not exist");
				
            asset.UpdateChange(command.HourChange, command.DayChange, command.WeekChange);
            _domainRepository.Save(asset);
        }
    }
}