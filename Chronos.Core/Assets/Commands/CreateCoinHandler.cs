using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class CreateCoinHandler : ICommandHandler<CreateCoinCommand>
    {
        private readonly IDomainRepository _domainRepository;

        public CreateCoinHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(CreateCoinCommand command)
        {
            if(_domainRepository.Exists<Coin>(command.TargetId))
                throw new InvalidOperationException("Coin has already been created");
                
            var coin = new Coin(command.TargetId,command.Ticker,command.Name);
            _domainRepository.Save(coin);
        }
    }    
}