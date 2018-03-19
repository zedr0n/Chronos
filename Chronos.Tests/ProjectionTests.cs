using System;
using Chronos.Core.Assets;
using Chronos.Core.Assets.Commands;
using Chronos.Core.Assets.Projections;
using Chronos.Core.Assets.Queries;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;
using Xunit;
using Xunit.Abstractions;

namespace Chronos.Tests
{
    public class ProjectionTests : ReadTestBase
    {
        public ProjectionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CanProjectHistory()
        {
            var container = CreateContainer(nameof(CanProjectHistory));
            var navigator = container.GetInstance<ITimeNavigator>();
            var handler = container.GetInstance<IQueryHandler<BagHistoryQuery, BagHistory>>();

            var commandBus = container.GetInstance<ICommandBus>();
            
            var coinId = Guid.NewGuid();
            var createCoinCommand = new CreateCoinCommand()
            {
                Name = "Bitcoin",
                TargetId = coinId,
                Ticker = "BTC"
            };
            commandBus.Send(createCoinCommand);

            var bagId = Guid.NewGuid();
            var createBagCommand = new CreateBagCommand("BTC")
            {
                TargetId = bagId
            };
            commandBus.Send(createBagCommand);
            
            var addAssetCommand = new AddAssetToBagCommand(coinId,1.0)
            {
                TargetId = bagId
            };
            commandBus.Send(addAssetCommand);

            for (var i = 0; i < 100; ++i)
            {
                var priceCommand = new UpdateAssetPriceCommand<Coin>
                {
                    Price = i,
                    TargetId = coinId
                };
                commandBus.Send(priceCommand);
            }
            
            //navigator.Reset();
            
            var query = new BagHistoryQuery(bagId);

            var bagHistory = handler.Handle(query);
            Assert.NotNull(bagHistory);
            Assert.Equal(99, bagHistory.Values.Count);
        }
    }
}