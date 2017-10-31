using System;
using System.Reactive.Linq;
using Chronos.Core.Assets.Commands;
using Chronos.Core.Assets.Projections;
using Chronos.Core.Assets.Queries;
using Chronos.Core.Common.Events;
using Chronos.Core.Net.Tracking.Commands;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;
using Chronos.Persistence;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Chronos.Tests
{
    public class AssetTests : TestBase
    {
        public AssetTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CanTrackBitcoin()
        {
            var container = CreateContainer(nameof(CanTrackBitcoin));
            var commandBus = container.GetInstance<ICommandBus>();
            var queryProcessor = container.GetInstance<IQueryProcessor>();
            var eventStore = container.GetInstance<IEventStore>();

            var coinId = Guid.NewGuid();
            
            commandBus.Send(new CreateCoinCommand
            {
                Name = "Bitcoin",
                TargetId = coinId,
                Ticker = "BTC"
            });
            
            commandBus.Send(
                new TrackCoinCommand(coinId,Duration.FromSeconds(2)) { Ticker = "Bitcoin"} );

            var query = new CoinInfoQuery
            {
                CoinId = coinId
            };

            var alerts = eventStore.Alerts.Publish();
            alerts.Connect();

            var obs = Observable.Interval(TimeSpan.FromSeconds(1))
                .TakeUntil(alerts.OfType<CoinInfoParsed>());
            obs.Wait();
            
            var coinInfo = queryProcessor.Process<CoinInfoQuery, CoinInfo>(query);
            Assert.NotNull(coinInfo);
            Assert.True(coinInfo.Price > 0);
        }
    }
}