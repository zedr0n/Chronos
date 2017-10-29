using Chronos.Core.Assets.Projections;
using Chronos.Core.Assets.Queries;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Assets.Commands
{
    public class CreateCoinHandler : ICommandHandler<CreateCoinCommand>
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IQueryHandler<CoinInfoQuery,CoinInfo> _handler;

        public CreateCoinHandler(IDomainRepository domainRepository, IQueryHandler<CoinInfoQuery, CoinInfo> handler)
        {
            _domainRepository = domainRepository;
            _handler = handler;
        }

        public void Handle(CreateCoinCommand command)
        {
            var coinInfo = _handler.Handle(new CoinInfoQuery
            {
                Name = command.Name
            });

            // the coin has alreqdy been created
            // what happens if create coin command was sent
            // but the projections weren't refreshed?
            // although saving events and sending them to subscribers
            // is supposed to be atomic as it's singlethreaded
            if (coinInfo != null)
                return;
                
            var coin = new Coin(command.TargetId,command.Ticker,command.Name);
            _domainRepository.Save(coin);
        }
    }    
}