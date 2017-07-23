using System;
using System.Threading;
using Chronos.Core.Accounts;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
using Chronos.Core.Transactions;
using Chronos.Core.Transactions.Commands;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Chronos.Tests
{
    public class IntegrationTests : TestBase
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
            var container = CreateContainer(nameof(CanCreateMultipleAccounts));
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
            var container = CreateContainer(nameof(CanProjectAccountInfo));
            var bus = container.GetInstance<ICommandBus>();

            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                AggregateId = id,
                Currency = "GBP",
                Name = "Account"
            };

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

            var container = CreateContainer(nameof(CanChangeAccount));
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
            Assert.Equal(-100, accountInfo.Balance);
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

            var container = CreateContainer(nameof(CanGetAccountInfoAsOf));
            var bus = container.GetInstance<ICommandBus>();
            bus.Send(createAccountCommand);    

            var id = Guid.NewGuid();

            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();
            var createdAt = queryHandler.Handle(new GetAccountInfo { AccountId = accountId }).CreatedAt;

            var command = new CreatePurchaseCommand
            {
                AggregateId = id,
                AccountId = accountId,
                Amount = 100,
                Currency = "GBP",
                Payee = "Payee"
            };

            bus.Send(command);
            var baseQuery = new GetAccountInfo { AccountId = accountId, AsOf = createdAt};
            var query = new GetAccountInfo { AccountId = accountId };

            var baseInfo = queryHandler.Handle(baseQuery);
            var lastInfo = queryHandler.Handle(query);

            Assert.Equal(0, baseInfo.Balance);
            Assert.Equal(-100, lastInfo.Balance);
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
            var container = CreateContainer(nameof(CanReplayEvents));

            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                AggregateId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

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

            Assert.Equal(-100, queryHandler.Handle(query).Balance);

            var navigator = container.GetInstance<ITimeNavigator>();
            navigator.Reset();

            var accountInfo = queryHandler.Handle(query);
            Assert.Equal(-100, accountInfo.Balance);
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

            Assert.Equal(-command.Amount, queryHandler.Handle(query).Balance);

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
            var retries = 0;
            var timer = new Timer(obj =>
            {
                retries++;
                if (repository.Find<Account>(id) != null || retries > 5)
                    waitHandle.Set();
            } , null, 500,500);

            waitHandle.WaitOne();
            Assert.True(repository.Find<Account>(id) != null);
            timer.Dispose();
        }

        [Fact]
        public void CanScheduleCommandInHistoricalMode()
        {
            var container = CreateContainer(nameof(CanScheduleCommand));
            var repository = container.GetInstance<IDomainRepository>();
            var bus = container.GetInstance<ICommandBus>();
            var timeNavigator = container.GetInstance<ITimeNavigator>();
            var pastDate = new ZonedDateTime(new LocalDateTime(2017, 07, 08, 0, 0), DateTimeZone.Utc, Offset.Zero).ToInstant();
            timeNavigator.GoTo(pastDate);

            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                AggregateId = id,
                Currency = "GBP",
                Name = "Account"
            };

            var scheduledOn = pastDate.Plus(Duration.FromDays(1));

            var scheduleGuid = Guid.NewGuid();

            var scheduleCommand = new ScheduleCommand
            {
                ScheduleId = scheduleGuid,
                Command = command,
                Date = scheduledOn
            };

            bus.Send(scheduleCommand);

            timeNavigator.Advance(Duration.FromHours(6));

            Assert.Throws<InvalidOperationException>(() => repository.Get<Account>(id));

            timeNavigator.Advance(Duration.FromDays(1));

            var waitHandle = new ManualResetEvent(false);
            var timer = new Timer(obj =>
            {
                if (repository.Find<Account>(id) != null)
                    waitHandle.Set();
            }, null, 500, 500);

            waitHandle.WaitOne();
            timer.Dispose();
        }

        [Fact]
        public void CanCreateTransfer()
        {
            var container = CreateContainer(nameof(CanCreatePurchase));
            var bus = container.GetInstance<ICommandBus>();

            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                AggregateId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            bus.Send(createAccountCommand);

            var otherAccountId = Guid.NewGuid();
            var createOtherAccountCommand = new CreateAccountCommand
            {
                AggregateId = otherAccountId,
                Currency = "GBP",
                Name = "OtherAccount"
            };

            bus.Send(createOtherAccountCommand);

            var transferId = Guid.NewGuid();

            var transferCommand = new CreateCashTransferCommand
            {
                AggregateId = transferId,
                Amount = 100,
                Currency = "GBP",
                FromAccount = accountId,
                ToAccount = otherAccountId
            };

            bus.Send(transferCommand);

            var query = new GetAccountInfo { AccountId = accountId };
            var otherQuery = new GetAccountInfo {AccountId = otherAccountId};

            var queryHandler = container.GetInstance<IQueryHandler<GetAccountInfo, AccountInfo>>();
            var accountInfo = queryHandler.Handle(query);
            var otherAccountInfo = queryHandler.Handle(otherQuery);

            Assert.Equal(-100, accountInfo.Balance);
            Assert.Equal(100, otherAccountInfo.Balance);
        }

        public IntegrationTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}