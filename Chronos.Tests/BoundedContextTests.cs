using System;
using System.Linq;
using Chronos.Core.Accounts;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
using Chronos.Core.Transactions;
using Chronos.Core.Transactions.Commands;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;
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

            var query = new GetAccountInfo { AccountId = id };

            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();
            var accountInfo = queryHandler.Handle(query);

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

            var query = new GetAccountInfo { AccountId = accountId };

            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();
            var accountInfo = queryHandler.Handle(query);

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

            var baseQuery = new GetAccountInfo { AccountId = accountId, AsOf = date};
            var query = new GetAccountInfo { AccountId = accountId };

            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();
            var baseInfo = queryHandler.Handle(baseQuery);
            var lastInfo = queryHandler.Handle(query);

            Assert.Equal(0, baseInfo.Balance);
            Assert.Equal(100, lastInfo.Balance);
        }

    }
}