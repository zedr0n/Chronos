using System;
using System.Runtime.InteropServices;
using System.Threading;
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
using NodaTime;
using Xunit;

namespace Chronos.Tests
{
    public class IntegrationTests : TestsBase
    {
        [Fact]
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

            var bus = container.GetInstance<ICommandBus>();
            bus.Send(command);

            var repository = container.GetInstance<IDomainRepository>();
            repository.Get<Account>(id);
        }

        [Fact]
        public void CanCreateMultipleAccounts()
        {
            var container = CreateContainer(nameof(CanCreateAccount));
            var bus = container.GetInstance<ICommandBus>();
            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();

            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                AggregateId = id,
                Currency = "GBP",
                Name = "Account"
            };

            bus.Send(command);

            var otherId = Guid.NewGuid();
            var otherCommand = new CreateAccountCommand
            {
                AggregateId = otherId,
                Currency = "GBP",
                Name = "OtherAccount"
            };

            bus.Send(otherCommand);

            var query = new GetAccountInfo { AccountId = id };
            var otherQuery = new GetAccountInfo {AccountId = otherId };

            var accountInfo = queryHandler.Handle(query);
            Assert.Equal("Account",accountInfo.Name);
            var otherAccountInfo = queryHandler.Handle(otherQuery);
            Assert.Equal("OtherAccount",otherAccountInfo.Name);

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
            var bus = container.GetInstance<ICommandBus>();
            bus.Send(command);

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
            var bus = container.GetInstance<ICommandBus>();
            bus.Send(command);
            
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
            bus.Send(changeCommand);

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
            var bus = container.GetInstance<ICommandBus>();
            bus.Send(createAccountCommand);

            var id = Guid.NewGuid();

            var command = new CreatePurchaseCommand
            {
                AggregateId = id,
                AccountId = accountId,
                Amount = 100,
                Currency = "GBP",
                Payee = "Payee"
            };
            bus.Send(command);

            var repository = container.GetInstance<IDomainRepository>();
            var purchase = repository.Find<Purchase>(id);

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
            var bus = container.GetInstance<ICommandBus>();
            bus.Send(createAccountCommand);
    
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
            var bus = container.GetInstance<ICommandBus>();
            bus.Send(createAccountCommand);    

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

            bus.Send(command);
            var baseQuery = new GetAccountInfo { AccountId = accountId, AsOf = acountCreationTime};
            var query = new GetAccountInfo { AccountId = accountId };

            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();
            var baseInfo = queryHandler.Handle(baseQuery);
            var lastInfo = queryHandler.Handle(query);

            Assert.Equal(0, baseInfo.Balance);
            Assert.Equal(100, lastInfo.Balance);
        }

        [Fact]
        public void CanCreateAccountInThePast()
        {
            var container = CreateContainer(nameof(CanCreateAccountInThePast));

            var navigator = container.GetInstance<ITimeNavigator>();            
            var pastDate = new ZonedDateTime(new LocalDateTime(2017,07,08,0,0), DateTimeZone.Utc,Offset.Zero).ToInstant();
            navigator.GoTo(pastDate);

            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                AggregateId = id,
                Currency = "GBP",
                Name = "Account"
            };

            var bus = container.GetInstance<ICommandBus>();
            bus.Send(command);

            var query = new GetAccountInfo { AccountId = id };

            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();
            var accountInfo = queryHandler.Handle(query);

            Assert.Equal(pastDate,accountInfo.CreatedAt);
        }

        [Fact]
        public void CanReplayEvents()
        {
            var container = CreateContainer(nameof(CanReplayEventsUpToPointInPast));

            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                AggregateId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            container.GetInstance<AccountInfoProjector>();

            var bus = container.GetInstance<ICommandBus>();
            bus.Send(createAccountCommand);
            var id = Guid.NewGuid();

            var query = new GetAccountInfo { AccountId = accountId };
            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();

            var command = new CreatePurchaseCommand
            {
                AggregateId = id,
                AccountId = accountId,
                Amount = 100,
                Currency = "GBP",
                Payee = "Payee"
            };
            bus.Send(command);

            Assert.Equal(command.Amount, queryHandler.Handle(query).Balance);

            var navigator = container.GetInstance<ITimeNavigator>();
            navigator.Reset();

            var accountInfo = queryHandler.Handle(query);
            Assert.Equal(200, accountInfo.Balance);
        }

        [Fact]
        public void CanReplayEventsUpToPointInPast()
        {
            var container = CreateContainer(nameof(CanReplayEventsUpToPointInPast));

            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                AggregateId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            container.GetInstance<AccountInfoProjector>();

            var bus = container.GetInstance<ICommandBus>();
            bus.Send(createAccountCommand);
            var id = Guid.NewGuid();

            var query = new GetAccountInfo { AccountId = accountId };
            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();
            var createdAt = queryHandler.Handle(query).CreatedAt;

            var command = new CreatePurchaseCommand
            {
                AggregateId = id,
                AccountId = accountId,
                Amount = 100,
                Currency = "GBP",
                Payee = "Payee"
            };
            bus.Send(command);

            Assert.Equal(command.Amount, queryHandler.Handle(query).Balance);

            var navigator = container.GetInstance<ITimeNavigator>();
            navigator.GoTo(createdAt);

            var accountInfo = queryHandler.Handle(query);
            Assert.Equal(0,accountInfo.Balance);

            navigator.Reset();
        }

        [Fact]
        public void CanScheduleCommand()
        {
            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                AggregateId = id,
                Currency = "GBP",
                Name = "Account"
            };

            var scheduledOn = Clock.GetCurrentInstant().Plus(Duration.FromSeconds(0.5));

            var scheduleCommand = new ScheduleCommand
            {
                AggregateId = id,
                Command = command,
                Date = scheduledOn
            };

            var container = CreateContainer(nameof(CanScheduleCommand));
            var repository = container.GetInstance<IDomainRepository>();
            var bus = container.GetInstance<ICommandBus>();
            bus.Send(scheduleCommand);

            Assert.Throws<InvalidOperationException>(() => repository.Get<Account>(id));

            var waitHandle = new ManualResetEvent(false);
            var timer = new Timer(obj =>
            {
                if (repository.Find<Account>(id) != null)
                    waitHandle.Set();
            } , null, 500,500);

            waitHandle.WaitOne();

            repository.Get<Account>(id);
        }
    }
}