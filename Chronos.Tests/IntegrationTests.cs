using System;
using System.Threading;
using Chronos.Core.Accounts;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
using Chronos.Core.Projections;
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
                TargetId = id,
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
            var processor = container.GetInstance<IQueryProcessor>();

            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                TargetId = id,
                Currency = "GBP",
                Name = "Account"
            };

            bus.Send(command);

            var otherId = Guid.NewGuid();
            var otherCommand = new CreateAccountCommand
            {
                TargetId = otherId,
                Currency = "GBP",
                Name = "OtherAccount"
            };

            bus.Send(otherCommand);

            var query = new AccountInfoQuery { AccountId = id };
            var otherQuery = new AccountInfoQuery {AccountId = otherId };

            var accountInfo = processor.Process<AccountInfoQuery,AccountInfo>(query);
            Assert.Equal("Account",accountInfo.Name);
            var otherAccountInfo = processor.Process<AccountInfoQuery,AccountInfo>(otherQuery);
            Assert.Equal("OtherAccount",otherAccountInfo.Name);

            var stats = processor.Process<StatsQuery, Stats>(new StatsQuery());
            Assert.Equal(2,stats.NumberOfAccounts);
        }

        [Fact]
        public void CanProjectAccountInfo()
        {
            var container = CreateContainer(nameof(CanProjectAccountInfo));
            var bus = container.GetInstance<ICommandBus>();

            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                TargetId = id,
                Currency = "GBP",
                Name = "Account"
            };

            bus.Send(command);

            var query = new AccountInfoQuery { AccountId = id };

            var queryHandler = container.GetInstance<IQueryHandler<AccountInfoQuery, AccountInfo>>();
            var accountInfo = queryHandler.Handle(query);

            Assert.Equal("Account",accountInfo.Name);
            Assert.Equal("GBP", accountInfo.Currency);
        }

        [Fact]
        public void CanProjectTotalMovement()
        {
            var container = CreateContainer(nameof(CanProjectTotalMovement));
            var bus = container.GetInstance<ICommandBus>();

            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                TargetId = id,
                Currency = "GBP",
                Name = "Account"
            };

            bus.Send(command);

            var query = new TotalMovementQuery();

            var queryHandler = container.GetInstance<IQueryHandler<TotalMovementQuery, TotalMovement>>();
            var movement = queryHandler.Handle(query);

            Assert.Equal(0, movement.Value);
        }

        [Fact]
        public void ThrowsOnChangingNonexistentAccount()
        {
            var command = new ChangeAccountCommand
            {
                TargetId = Guid.NewGuid(),
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
                TargetId = id,
                Currency = "GBP",
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanChangeAccount));
            var bus = container.GetInstance<ICommandBus>();
            bus.Send(command);
            
            var query = new AccountInfoQuery {AccountId = id};
            var queryHandler = container.GetInstance<IQueryHandler<AccountInfoQuery, AccountInfo>>();
            var accountInfo = queryHandler.Handle(query);

            Assert.Equal("Account", accountInfo.Name);

            var changeCommand = new ChangeAccountCommand
            {
                TargetId = id,
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
                TargetId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanCreatePurchase));
            var bus = container.GetInstance<ICommandBus>();
            bus.Send(createAccountCommand);

            var id = Guid.NewGuid();

            var command = new CreatePurchaseCommand
            {
                TargetId = id,
                AccountId = accountId,
                Amount = 100,
                Currency = "GBP",
                Payee = "Payee"
            };
            bus.Send(command);

            var repository = container.GetInstance<IDomainRepository>();
            var purchase = repository.Find<Purchase>(id);

            var query = new AccountInfoQuery { AccountId = accountId };

            var queryHandler = container.GetInstance<IQueryHandler<AccountInfoQuery, AccountInfo>>();
            var accountInfo = queryHandler.Handle(query);

            Assert.NotNull(purchase);
            Assert.Equal(-100, accountInfo.Balance);
        }
        
        [Fact]
        public void CanGetTotalMovementFromTransaction()
        {
            var container = CreateContainer(nameof(CanGetTotalMovementFromTransaction));
            var bus = container.GetInstance<ICommandBus>();

            var accountId = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                TargetId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            bus.Send(command);

            var accountInfoQuery = new AccountInfoQuery
            {
                AccountId = accountId
            };

            var accountInfoHandler = container.GetInstance<IQueryHandler<AccountInfoQuery, AccountInfo>>();
            var createdAt = accountInfoHandler.Handle(accountInfoQuery).CreatedAt;

            var transactionId = Guid.NewGuid();
            
            var transactionCommand = new CreatePurchaseCommand
            {
                TargetId = transactionId,
                AccountId = accountId,
                Amount = 100,
                Currency = "GBP",
                Payee = "Payee"
            };
            bus.Send(transactionCommand);
            
            var query = new TotalMovementQuery();

            var queryHandler = container.GetInstance<IQueryHandler<TotalMovementQuery, TotalMovement>>();
            var movement = queryHandler.Handle(query);
            
            var historicalQuery = new HistoricalQuery<TotalMovementQuery>()
            {
                Query = query,
                AsOf = createdAt
            };

            var historicalQueryHandler =
                container.GetInstance<IHistoricalQueryHandler<TotalMovementQuery,TotalMovement>>();

            var initialMovement = historicalQueryHandler.Handle(historicalQuery);
            Assert.Equal(0, initialMovement.Value);
            Assert.Equal(100, movement.Value);
        }

        [Fact]
        public void CanGetAccountInfoAsOf()
        {
            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                TargetId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanGetAccountInfoAsOf));
            var bus = container.GetInstance<ICommandBus>();
            bus.Send(createAccountCommand);    

            var id = Guid.NewGuid();

            var queryHandler = container.GetInstance<IQueryHandler<AccountInfoQuery, AccountInfo>>();
            var historicalQueryHandler =
                container.GetInstance<IHistoricalQueryHandler<AccountInfoQuery, AccountInfo>>();
                //container.GetInstance<IQueryHandler<HistoricalQuery<AccountInfoQuery>,AccountInfo>>();
            var createdAt = queryHandler.Handle(new AccountInfoQuery { AccountId = accountId }).CreatedAt;

            var command = new CreatePurchaseCommand
            {
                TargetId = id,
                AccountId = accountId,
                Amount = 100,
                Currency = "GBP",
                Payee = "Payee"
            };

            bus.Send(command);
            var baseQuery = new AccountInfoQuery { AccountId = accountId };
            var historicalQuery = new HistoricalQuery<AccountInfoQuery>
            {
                Query = baseQuery,
                AsOf = createdAt
            };

            var asOfInfo = historicalQueryHandler.Handle(historicalQuery);
            var lastInfo = queryHandler.Handle(baseQuery);

            Assert.Equal(0, asOfInfo.Balance);
            Assert.Equal(-100, lastInfo.Balance);
        }

        [Fact]
        public void CanStartTimelineFromEmpty()
        {
            var container = CreateContainer(nameof(CanStartTimelineFromEmpty));

            var navigator = container.GetInstance<ITimeNavigator>();            
            var pastDate = new ZonedDateTime(new LocalDateTime(2017,07,08,0,0), DateTimeZone.Utc,Offset.Zero).ToInstant();
            navigator.GoTo(pastDate);

            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                TargetId = id,
                Currency = "GBP",
                Name = "Account"
            };

            var bus = container.GetInstance<ICommandBus>();
            bus.Send(command);

            var query = new AccountInfoQuery { AccountId = id };

            var queryHandler = container.GetInstance<IQueryHandler<AccountInfoQuery, AccountInfo>>();
            var accountInfo = queryHandler.Handle(query);

            Assert.Equal(pastDate,accountInfo.CreatedAt);
            
            navigator.Reset();
            var mainAccountInfo = queryHandler.Handle(query);
            Assert.Null(mainAccountInfo);
        }

        [Fact]
        public void CanReplayEvents()
        {
            var container = CreateContainer(nameof(CanReplayEvents));

            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                TargetId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            var bus = container.GetInstance<ICommandBus>();
            bus.Send(createAccountCommand);
            var id = Guid.NewGuid();

            var query = new AccountInfoQuery { AccountId = accountId };
            var queryHandler = container.GetInstance<IQueryHandler<AccountInfoQuery, AccountInfo>>();

            var command = new CreatePurchaseCommand
            {
                TargetId = id,
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
        public void CanCreateTimelineFromDate()
        {
            var container = CreateContainer(nameof(CanCreateTimelineFromDate));

            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                TargetId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            var bus = container.GetInstance<ICommandBus>();
            bus.Send(createAccountCommand);
            var id = Guid.NewGuid();

            var query = new AccountInfoQuery { AccountId = accountId };
            var queryHandler = container.GetInstance<IQueryHandler<AccountInfoQuery, AccountInfo>>();
            var createdAt = queryHandler.Handle(query).CreatedAt;

            var command = new CreatePurchaseCommand
            {
                TargetId = id,
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
                TargetId = id,
                Currency = "GBP",
                Name = "Account"
            };

            var scheduledOn = Clock.GetCurrentInstant().Plus(Duration.FromSeconds(0.1));

            var scheduleCommand = new ScheduleCommand
            {
                ScheduleId = Guid.NewGuid(),
                TargetId = id,
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
                if (repository.Find<Account>(id) != null /* || retries > 5 */ )
                    waitHandle.Set();
            } , null, 100,100);

            waitHandle.WaitOne();
            Assert.True(repository.Find<Account>(id) != null);
            timer.Dispose();
        }

        [Fact]
        public void CanScheduleCommandInHistoricalMode()
        {
            var container = CreateContainer(nameof(CanScheduleCommandInHistoricalMode));
            var repository = container.GetInstance<IDomainRepository>();
            var bus = container.GetInstance<ICommandBus>();
            var timeNavigator = container.GetInstance<ITimeNavigator>();
            var pastDate = new ZonedDateTime(new LocalDateTime(2017, 07, 08, 0, 0), DateTimeZone.Utc, Offset.Zero).ToInstant();
            timeNavigator.GoTo(pastDate);

            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                TargetId = id,
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
                TargetId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            bus.Send(createAccountCommand);

            var otherAccountId = Guid.NewGuid();
            var createOtherAccountCommand = new CreateAccountCommand
            {
                TargetId = otherAccountId,
                Currency = "GBP",
                Name = "OtherAccount"
            };

            bus.Send(createOtherAccountCommand);

            var transferId = Guid.NewGuid();

            var transferCommand = new CreateCashTransferCommand
            {
                TargetId = transferId,
                Amount = 100,
                Currency = "GBP",
                FromAccount = accountId,
                ToAccount = otherAccountId
            };

            bus.Send(transferCommand);

            var query = new AccountInfoQuery { AccountId = accountId };
            var otherQuery = new AccountInfoQuery {AccountId = otherAccountId};

            var queryHandler = container.GetInstance<IQueryHandler<AccountInfoQuery, AccountInfo>>();
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