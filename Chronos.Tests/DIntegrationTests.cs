using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Chronos.Core.Accounts;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Events;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
using Chronos.Core.Transactions;
using Chronos.Core.Transactions.Commands;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Queries;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Chronos.Tests
{

    public class DIntegrationTests : DTestBase
    {
        public DIntegrationTests(ITestOutputHelper output) : base(output)
        {
            
        }

        private static class History
        {
            public static readonly Guid AccountId = Guid.NewGuid();
            public static readonly Guid OtherAccountId = Guid.NewGuid();

            public static readonly AccountCreated AccountCreated = new AccountCreated
            {
                AccountId = AccountId,
                Currency = "GBP",
                Name = "Account"
            };

            public static readonly AccountChanged AccountChanged = new AccountChanged
            {
                AccountId = AccountId,
                Currency = "GBP",
                Name = "OtherAccount"
            };

            public static readonly AccountCreated OtherAccountCreated = new AccountCreated
            {
                AccountId = OtherAccountId,
                Currency = "GBP",
                Name = "OtherAccount"
            };

            public static readonly Guid PurchaseId = Guid.NewGuid();
            
            public static readonly PurchaseCreated PurchaseCreated = new PurchaseCreated
            {
                AccountId = AccountId,
                PurchaseId = PurchaseId,
                Amount = 100,
                Currency = "GBP",
                Payee = "Payee"
            };

            public static readonly CashWithdrawn CashWithdrawn = new CashWithdrawn
            {
                AccountId = AccountId,
                Amount = 100
            };
        }
        
        [Fact]
        public void CanCreateAccount()
        { 
            GetInstance<Specification>().When(
                new CreateAccountCommand
                {
                    TargetId = History.AccountId,
                    Currency = "GBP",
                    Name = "Account"
                }).Then(History.AccountCreated);
        }

        [Fact]
        public void CanChangeAccount()
        {
            var spec = GetInstance<Specification>();
            spec.Given<Account>(History.AccountId, History.AccountCreated)
                .When(new ChangeAccountCommand
                {
                    TargetId = History.AccountId,
                    Name = "OtherAccount",
                    Currency = "GBP"
                })
                .Then(History.AccountChanged);
        }

        [Fact]
        public void CanCreateMultipleAccounts()
        {
            var spec = GetInstance<Specification>();
            spec.When(new CreateAccountCommand
                {
                    TargetId = History.AccountId,
                    Currency = "GBP",
                    Name = "Account"
                })
                .When(new CreateAccountCommand
                {
                    TargetId = History.OtherAccountId,
                    Currency = "GBP",
                    Name = "OtherAccount"
                })
                .Then(History.AccountCreated, History.OtherAccountCreated);
        }

        [Fact]
        public void CanProjectAccountInfo()
        {
            var spec = GetInstance<Specification>();
            var accountInfo = spec.Given<Account>(History.AccountId, History.AccountCreated)
                .Query<AccountInfoQuery,AccountInfo>(new AccountInfoQuery
                {
                    AccountId = History.AccountId
                });
            Assert.Equal("Account",accountInfo.Name);
            Assert.Equal("GBP",accountInfo.Currency);
        }

        [Fact]
        public void CanProjectTotalMovement()
        {
            var spec = GetInstance<Specification>();
            var total = spec.Given<Account>(History.AccountId, History.AccountCreated)
                .Query<TotalMovementQuery, TotalMovement>(new TotalMovementQuery());
            
            Assert.Equal(0, total.Value);
        }

        [Fact]
        public void ThrowsOnChangingNonexistentAccount()
        {
            var spec = GetInstance<Specification>();
            var exception = Record.Exception(() => 
                spec.When(new ChangeAccountCommand
                {
                    TargetId = History.AccountId,
                    Name = "Account",
                    Currency = "GBP"
                }));
            Assert.IsType<InvalidOperationException>(exception?.InnerException);
        }

        [Fact]
        public void CanCreatePurchase()
        {
            var spec = GetInstance<Specification>();
            spec.Given<Account>(History.AccountId, History.AccountCreated)
                .When(new CreatePurchaseCommand
                {
                    AccountId = History.AccountId,
                    TargetId = History.PurchaseId,
                    Currency = "GBP",
                    Amount = 100,
                    Payee = "Payee"
                })
                .Then(History.PurchaseCreated, History.CashWithdrawn);
        }

        [Fact]
        public void CanGetTotalMovementFromTransaction()
        {
            var spec = GetInstance<Specification>();
            var movement = spec
                .Given<Account>(History.AccountId, History.AccountCreated)
                .When(new CreatePurchaseCommand
                {
                    AccountId = History.AccountId,
                    TargetId = History.PurchaseId,
                    Currency = "GBP",
                    Amount = 100,
                    Payee = "Payee"
                })
                .Query<TotalMovementQuery, TotalMovement>(new TotalMovementQuery());
            
            Assert.Equal(100, movement.Value);
        }

        [Fact]
        public void CanGetAccountInfoAsOf()
        {
            var spec = GetInstance<Specification>();
            spec.Given<Account>(History.AccountId, History.AccountCreated)
                .Given<Purchase>(History.PurchaseId, History.PurchaseCreated);
            var createdAt = spec.Query<AccountInfoQuery,AccountInfo>(new
                AccountInfoQuery
                {
                    AccountId = History.AccountId
                })
                .CreatedAt;

            var historicalInfo = spec.Query<AccountInfoQuery, AccountInfo>(new HistoricalQuery<AccountInfoQuery>
            {
                AsOf = createdAt,
                Query = new AccountInfoQuery
                {
                    AccountId = History.AccountId
                }
            });
            
            Assert.Equal(0,historicalInfo.Balance);
        }

        [Fact]
        public void CanCreateAccountInThePast()
        {
            var spec = GetInstance<Specification>();
            var pastDate = new ZonedDateTime(new LocalDateTime(2017,07,08,0,0), DateTimeZone.Utc,Offset.Zero).ToInstant();
            var accountInfo = spec.At(pastDate)
                .When(new CreateAccountCommand
                {
                    TargetId = History.AccountId,
                    Currency = "GBP",
                    Name = "Account"
                })
                .Query<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
                {
                    AccountId = History.AccountId
                });
            Assert.Equal(pastDate,accountInfo.CreatedAt);
        }

        [Fact]
        public void CanReplayEventsUpToPointInPast()
        {
            var spec = GetInstance<Specification>();
            spec.Given<Account>(History.AccountId, History.AccountCreated)
                .Given<Purchase>(History.PurchaseId, History.PurchaseCreated);
            var createdAt = spec.Query<AccountInfoQuery,AccountInfo>(new
                    AccountInfoQuery
                    {
                        AccountId = History.AccountId
                    })
                .CreatedAt;

            var accountInfo = spec.At(createdAt).Query<AccountInfoQuery,AccountInfo>(new AccountInfoQuery
            {
                AccountId = History.AccountId
            });
            
            Assert.Equal(0, accountInfo.Balance);
        }

        [Fact]
        public void CanScheduleCommand()
        {
            var spec = GetInstance<Specification>();
            var scheduledOn = Clock.GetCurrentInstant().Plus(Duration.FromSeconds(0.5));

            spec.When(new ScheduleCommand
            {
                ScheduleId = Guid.NewGuid(),
                TargetId = History.AccountId,
                Command = new CreateAccountCommand
                {
                    TargetId = History.AccountId,
                    Currency = "GBP",
                    Name = "Account"
                },
                Date = scheduledOn
            });

            var accountInfo = spec.Query<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
            {
                AccountId = History.AccountId
            });
            
            Assert.Null(accountInfo);
            
            var waitHandle = new ManualResetEvent(false);
            var retries = 0;
            var timer = new Timer(obj =>
            {
                retries++;
                if (spec.Has<Account>(History.AccountId) || retries > 5)
                    waitHandle.Set();
            } , null, 100,100);

            waitHandle.WaitOne();
            timer.Dispose();
            
            Assert.True(spec.Has<Account>(History.AccountId));
        }
    }
}