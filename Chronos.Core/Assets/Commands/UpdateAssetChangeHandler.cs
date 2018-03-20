using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class UpdateAssetChangeHandler<T> : ICommandHandler<UpdateAssetChangeCommand<T>>
        where T : class, IAsset,new()
    {
        private readonly IDomainRepository _domainRepository;

        public UpdateAssetChangeHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(UpdateAssetChangeCommand<T> command)
        {
            var asset = _domainRepository.Find<T>(command.TargetId);
            if(asset == null)
                throw new InvalidOperationException("Asset does not exist");
				
            asset.UpdateChange(command.HourChange, command.DayChange, command.WeekChange);
            _domainRepository.Save(asset);
        }
    }
}