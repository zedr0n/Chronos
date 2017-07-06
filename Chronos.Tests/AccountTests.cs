using System;
using Chronos.Core.Accounts;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Projections;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Projections;
using NodaTime;
using SimpleInjector;
using Xunit;

namespace Chronos.Tests
{
    public class AccountTests : TestsBase
    {
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

        [Fact]
        public void CanGetAccountInfoAsOf()
        {
            var id = Guid.NewGuid();
            var date = Clock.GetCurrentInstant();
            var prevDate = date.Minus(Duration.FromDays(1));
            var command = new CreateAccountCommand
            {
                Guid = id,
                Currency = "GBP",
                Date = date,
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanProjectAccountInfo));
            container.GetInstance<AccountInfoProjector>();

            var handler = container.GetInstance<ICommandHandler<CreateAccountCommand>>();
            handler.Handle(command);

            var repository = container.GetInstance<IRepository<AccountInfo>>();
            var accountInfo = repository.Get(command.Guid);

            Assert.Equal("Account", accountInfo.Name);

        }
    }
}