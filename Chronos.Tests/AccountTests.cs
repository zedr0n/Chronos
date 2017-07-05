using System;
using Chronos.Core.Account.Aggregates;
using Chronos.Core.Account.Commands;
using Chronos.Core.Account.Projections;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Projections;
using NodaTime;
using SimpleInjector;
using Xunit;

namespace Chronos.Tests
{
    public class AccountTests
    {
        private static readonly IClock Clock;

        static AccountTests()
        {
            Clock = SystemClock.Instance;

        }

        private static Container CreateContainer(string dbName)
        {
            var container = new Container();
            new CompositionRoot().ComposeApplication(container,dbName,false);
            container.Verify();
            return container;
        }

        [Fact]
        public void CanCreateAccount()
        {
            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                Guid = id,
                Currency = "GBP",
                Date = Clock.GetCurrentInstant(),
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanCreateAccount));

            var handler = container.GetInstance<ICommandHandler<CreateAccountCommand>>();
            handler.Handle(command);

            var repository = container.GetInstance<IDomainRepository>();
            repository.Get<Account>(id);
        }

        [Fact]
        public void CanProjectAccountInfo()
        {
            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                Guid = id,
                Currency = "GBP",
                Date = Clock.GetCurrentInstant(),
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanProjectAccountInfo));
            container.GetInstance<AccountInfoProjector>();

            var handler = container.GetInstance<ICommandHandler<CreateAccountCommand>>();
            handler.Handle(command);

            var repository = container.GetInstance<IRepository<AccountInfo>>();
            var accountInfo = repository.Get(command.Guid);

            Assert.Equal("Account",accountInfo.Name);
        }
    }
}