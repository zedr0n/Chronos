using System;
using System.Reactive.Linq;
using Chronos.Core.Coinbase.Commands;
using Chronos.Core.Coinbase.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Xunit;
using Xunit.Abstractions;

namespace Chronos.Tests
{
    public class CoinbaseTests : TestBase
    {
        public CoinbaseTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CanCreateCoinbaseAccount()
        {
            var container = CreateContainer(nameof(CanCreateCoinbaseAccount));
            var commandBus = container.GetInstance<ICommandBus>();
            var eventStore = container.GetInstance<IEventStore>();

            var accountCreated = false;

            eventStore.Events.OfType<CoinbaseAccountCreated>().Subscribe(e => accountCreated = true);
            
            var accountId = Guid.NewGuid();
            var command = new CreateCoinbaseAccountCommand("test@test.com")
            {
                TargetId = accountId
            };
            commandBus.Send(command);
            
            Assert.True(accountCreated);
        }

        [Fact]
        public void CanProcessCoinbasePurchase()
        {
            var container = CreateContainer(nameof(CanProcessCoinbasePurchase));
            var commandBus = container.GetInstance<ICommandBus>();
            var eventStore = container.GetInstance<IEventStore>();

            var coinPurchased = false;

            eventStore.Events.OfType<CoinPurchased>().Subscribe(e => coinPurchased = true);
            
            var accountId = Guid.NewGuid();
            var command = new CreateCoinbaseAccountCommand("test@test.com")
            {
                TargetId = accountId
            };
            commandBus.Send(command);

            var purchaseId = Guid.NewGuid();
            var purchaseCommand = new PurchaseCoinCommand(purchaseId, "Bitcoin",1.0,1000.0,3.0)
            {
                TargetId = accountId
            };
            commandBus.Send(purchaseCommand);
            
            Assert.True(coinPurchased);
        }
    }
}