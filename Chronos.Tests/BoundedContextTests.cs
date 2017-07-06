using System;
using System.Linq;
using Chronos.Core.Accounts;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Transactions;
using Chronos.Core.Transactions.Commands;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Projections;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using SimpleInjector;
using Xunit;

namespace Chronos.Tests
{
    public class BoundedContextTests : TestsBase
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
        public void CanCreatePurchase()
        {
            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                Guid = accountId,
                Currency = "GBP",
                Date = Clock.GetCurrentInstant(),
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanCreatePurchase));

            var handler = container.GetInstance<ICommandHandler<CreateAccountCommand>>();
            handler.Handle(createAccountCommand);

            var id = Guid.NewGuid();

            var command = new CreatePurchaseCommand
            {
                Id = id,
                AccountId = accountId,
                Amount = 100,
                Currency = "GBP",
                Date = Clock.GetCurrentInstant(),
                Payee = "Payee"
            };

            var handler2 = container.GetInstance<ICommandHandler<CreatePurchaseCommand>>();
            handler2.Handle(command);

            var repository = container.GetInstance<IDomainRepository>();
            var purchase = repository.Find<Purchase>(id);

            container.GetInstance<AccountInfoProjector>();

            var projectionRepository = container.GetInstance<IRepository<AccountInfo>>();
            var accountInfo = projectionRepository.Get(accountId);

            Assert.NotNull(purchase);
            Assert.Equal(100, accountInfo.Balance);
        }

        [Fact]
        public void CanGetAccountInfoAsOf()
        {
            var accountId = Guid.NewGuid();
            var date = Clock.GetCurrentInstant();
            var prevDate = date.Minus(Duration.FromDays(1));
            var createAccountCommand = new CreateAccountCommand
            {
                Guid = accountId,
                Currency = "GBP",
                Date = date,
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanProjectAccountInfo));
            container.GetInstance<AccountInfoProjector>();

            var handler = container.GetInstance<ICommandHandler<CreateAccountCommand>>();
            handler.Handle(createAccountCommand);
            var id = Guid.NewGuid();

            var command = new CreatePurchaseCommand
            {
                Id = id,
                AccountId = accountId,
                Amount = 100,
                Currency = "GBP",
                Date = Clock.GetCurrentInstant(),
                Payee = "Payee"
            };

            var handler2 = container.GetInstance<ICommandHandler<CreatePurchaseCommand>>();
            handler2.Handle(command);

            var repository = container.GetInstance<IRepository<AccountInfo>>();

            var projector = container.GetInstance<AccountInfoProjector>();
            projector.Rebuild(date);

            var baseInfo = repository.Get(createAccountCommand.Guid,date).Last();
            var lastInfo = repository.Get(createAccountCommand.Guid, Instant.MaxValue).Last();

            Assert.Equal(0, baseInfo.Balance);
            Assert.Equal(100, lastInfo.Balance);
        }

    }
}