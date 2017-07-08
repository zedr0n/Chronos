using System;
using Chronos.Core.Accounts;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
using Chronos.Core.Transactions;
using Chronos.Core.Transactions.Commands;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;
using Xunit;

namespace Chronos.Tests
{
    public class BoundedContextTests : TestsBase
    {
        [Fact,Trait("Category","AggregateTest")]
        public void CanCreateAccount()
        {
            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                AggregateId = id,
                Currency = "GBP",
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
                AggregateId = id,
                Currency = "GBP",
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
            Assert.Equal("GBP", accountInfo.Currency);
        }

        [Fact]
        public void ThrowsOnChangingNonexistentAccount()
        {
            var command = new ChangeAccountCommand
            {
                AggregateId = Guid.NewGuid(),
                Currency = "",
                Name = ""
            };

            var container = CreateContainer(nameof(ThrowsOnChangingNonexistentAccount));
            var handler = container.GetInstance<ICommandHandler<ChangeAccountCommand>>();
            Assert.Throws<InvalidOperationException>(() => handler.Handle(command));
        }

        [Fact]
        public void CanChangeAccount()
        {
            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                AggregateId = id,
                Currency = "GBP",
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanProjectAccountInfo));
            container.GetInstance<AccountInfoProjector>();

            var handler = container.GetInstance<ICommandHandler<CreateAccountCommand>>();
            handler.Handle(command);

            var query = new GetAccountInfo {AccountId = id};
            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();
            var accountInfo = queryHandler.Handle(query);

            Assert.Equal("Account", accountInfo.Name);

            var changeCommand = new ChangeAccountCommand
            {
                AggregateId = id,
                Name = "OtherAccount",
                Currency = "GBP"
            };
            container.GetInstance<ICommandHandler<ChangeAccountCommand>>().Handle(changeCommand);

            var nextAccountInfo = queryHandler.Handle(query);
            Assert.Equal("OtherAccount",nextAccountInfo.Name);
        }


        [Fact]
        public void CanCreatePurchase()
        {
            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                AggregateId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanCreatePurchase));

            var handler = container.GetInstance<ICommandHandler<CreateAccountCommand>>();
            handler.Handle(createAccountCommand);

            var id = Guid.NewGuid();

            var command = new CreatePurchaseCommand
            {
                AggregateId = id,
                AccountId = accountId,
                Amount = 100,
                Currency = "GBP",
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
        public void CanRebuildAccountInfo()
        {
            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                AggregateId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanProjectAccountInfo));
            container.GetInstance<AccountInfoProjector>();

            var handler = container.GetInstance<ICommandHandler<CreateAccountCommand>>();
            handler.Handle(createAccountCommand);

            var query = new GetAccountInfo { AccountId = accountId };

            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();
            var accountInfo = queryHandler.Handle(query);

            Assert.Equal("Account", accountInfo.Name);

            var projectionManager = container.GetInstance<IProjectionManager>();
            projectionManager.Rebuild<AccountInfo>(x => x.AccountId == accountId);
            var otherInfo = queryHandler.Handle(query);
            Assert.True(accountInfo.LastEvent == otherInfo.LastEvent);
        }

        [Fact]
        public void CanGetAccountInfoAsOf()
        {
            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                AggregateId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanProjectAccountInfo));
            container.GetInstance<AccountInfoProjector>();

            var handler = container.GetInstance<ICommandHandler<CreateAccountCommand>>();
            handler.Handle(createAccountCommand);
            var id = Guid.NewGuid();

            var acountCreationTime = Clock.GetCurrentInstant();

            var command = new CreatePurchaseCommand
            {
                AggregateId = id,
                AccountId = accountId,
                Amount = 100,
                Currency = "GBP",
                Payee = "Payee"
            };

            var handler2 = container.GetInstance<ICommandHandler<CreatePurchaseCommand>>();
            handler2.Handle(command);

            var baseQuery = new GetAccountInfo { AccountId = accountId, AsOf = acountCreationTime};
            var query = new GetAccountInfo { AccountId = accountId };

            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();
            var baseInfo = queryHandler.Handle(baseQuery);
            var lastInfo = queryHandler.Handle(query);

            Assert.Equal(0, baseInfo.Balance);
            Assert.Equal(100, lastInfo.Balance);
        }

    }
}