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
using Chronos.Core.Assets;
using Chronos.Core.Assets.Commands;
using Chronos.Core.Assets.Events;
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

        private static readonly Guid AccountId = Guid.NewGuid();
        private static readonly Guid OtherAccountId = Guid.NewGuid();
        private static readonly Guid PurchaseId = Guid.NewGuid();
        private static readonly Guid ScheduleId = Guid.NewGuid();
        private static readonly Guid CoinId = Guid.NewGuid();
        
        private static class History
        {
            public static readonly AccountCreated AccountCreated = new AccountCreated
            {
                AccountId = AccountId,
                Currency = "GBP",
                Name = "Account"
            };

            public static readonly CoinCreated CoinCreated = new CoinCreated
            {
                CoinId = CoinId,
                Name = "Bitcoin",
                Ticker = "BTC"
            };

            public static readonly AssetPriceUpdated PriceUpdated = new AssetPriceUpdated
            {
                AssetId = CoinId,
                Price = 100
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
                    TargetId = AccountId,
                    Currency = "GBP",
                    Name = "Account"
                }).Then(History.AccountCreated);
        }

        [Fact]
        public void CanCreateCoin()
        {
            GetInstance<Specification>().When(
                new CreateCoinCommand
                {
                    TargetId = CoinId,
                    Name = "Bitcoin",
                    Ticker = "BTC"
                }).Then(History.CoinCreated);
        }

        [Fact]
        public void CanUpdatePrice()
        {
            GetInstance<Specification>()
                .Given<Coin>(CoinId, History.CoinCreated)
                .When(new UpdateAssetPriceCommand
                {
                    TargetId = CoinId,
                    Price = 100
                })
                .Then(History.PriceUpdated);
        }

        [Fact]
        public void CanChangeAccount()
        {
            var spec = GetInstance<Specification>();
            spec.Given<Account>(AccountId, History.AccountCreated)
                .When(new ChangeAccountCommand
                {
                    TargetId = AccountId,
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
                    TargetId = AccountId,
                    Currency = "GBP",
                    Name = "Account"
                })
                .When(new CreateAccountCommand
                {
                    TargetId = OtherAccountId,
                    Currency = "GBP",
                    Name = "OtherAccount"
                })
                .Then(History.AccountCreated, History.OtherAccountCreated);
        }

        [Fact]
        public void CanProjectAccountInfo()
        {
            var spec = GetInstance<Specification>();
            var accountInfo = spec.Given<Account>(AccountId, History.AccountCreated)
                .Query<AccountInfoQuery,AccountInfo>(new AccountInfoQuery
                {
                    AccountId = AccountId
                });
            Assert.Equal("Account",accountInfo.Name);
            Assert.Equal("GBP",accountInfo.Currency);
        }

        [Fact]
        public void CanProjectTotalMovement()
        {
            var spec = GetInstance<Specification>();
            var total = spec.Given<Account>(AccountId, History.AccountCreated)
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
                    TargetId = AccountId,
                    Name = "Account",
                    Currency = "GBP"
                }));
            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void CanCreatePurchase()
        {
            var spec = GetInstance<Specification>();
            spec.Given<Account>(AccountId, History.AccountCreated)
                .When(new CreatePurchaseCommand
                {
                    AccountId = AccountId,
                    TargetId = PurchaseId,
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
                .Given<Account>(AccountId, History.AccountCreated)
                .When(new CreatePurchaseCommand
                {
                    AccountId = AccountId,
                    TargetId = PurchaseId,
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
            spec.Given<Account>(AccountId, History.AccountCreated)
                .Given<Purchase>(PurchaseId, History.PurchaseCreated);
            var createdAt = spec.Query<AccountInfoQuery,AccountInfo>(new
                AccountInfoQuery
                {
                    AccountId = AccountId
                })
                .CreatedAt;

            var historicalInfo = spec.Query<AccountInfoQuery, AccountInfo>(new HistoricalQuery<AccountInfoQuery>
            {
                AsOf = createdAt,
                Query = new AccountInfoQuery
                {
                    AccountId = AccountId
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
                    TargetId = AccountId,
                    Currency = "GBP",
                    Name = "Account"
                })
                .Query<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
                {
                    AccountId = AccountId
                });
            Assert.Equal(pastDate,accountInfo.CreatedAt);
        }

        [Fact]
        public void CanCreateAcountInThePastEx()
        {
            var spec = GetInstance<Specification>();
            var pastDate = new ZonedDateTime(new LocalDateTime(2017,07,08,0,0), DateTimeZone.Utc,Offset.Zero).ToInstant(); 
            var accountInfo = spec.When(new HistoricalCommand<CreateAccountCommand>
            {
                Command = new CreateAccountCommand
                {
                    TargetId = AccountId,
                    Currency = "GBP",
                    Name = "Account"
                },
                At = pastDate
            })
                .Query<AccountInfoQuery,AccountInfo>(new AccountInfoQuery
                {
                    AccountId = AccountId
                });
            Assert.Equal(pastDate,accountInfo.CreatedAt);
        }

        [Fact]
        public void CanReplayEventsUpToPointInPast()
        {
            var spec = GetInstance<Specification>();
            spec.Given<Account>(AccountId, History.AccountCreated)
                .Given<Purchase>(PurchaseId, History.PurchaseCreated);
            var createdAt = spec.Query<AccountInfoQuery,AccountInfo>(new
                    AccountInfoQuery
                    {
                        AccountId = AccountId
                    })
                .CreatedAt;

            var accountInfo = spec.At(createdAt).Query<AccountInfoQuery,AccountInfo>(new AccountInfoQuery
            {
                AccountId = AccountId
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
                TargetId = AccountId,
                Command = new CreateAccountCommand
                {
                    TargetId = AccountId,
                    Currency = "GBP",
                    Name = "Account"
                },
                Date = scheduledOn
            });

            var accountInfo = spec.Query<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
            {
                AccountId = AccountId
            });
            
            Assert.Null(accountInfo);
            
            var waitHandle = new ManualResetEvent(false);
            var retries = 0;
            var timer = new Timer(obj =>
            {
                retries++;
                if (spec.Has<Account>(AccountId) || retries > 20)
                    waitHandle.Set();
            } , null, 100,100);

            waitHandle.WaitOne();
            timer.Dispose();
            
            Assert.True(spec.Has<Account>(AccountId));
        }

        //[Fact]
        public void CanScheduleCommandInHistoricalMode()
        {
            var spec = GetInstance<Specification>();

            var pastDate = new ZonedDateTime(new LocalDateTime(2017, 07, 08, 0, 0), DateTimeZone.Utc, Offset.Zero)
                .ToInstant();
            var scheduledOn = pastDate.Plus(Duration.FromDays(1));
            spec.At(pastDate)
                .When(new ScheduleCommand
                {
                    ScheduleId = ScheduleId,
                    Command = new CreateAccountCommand
                    {
                        TargetId = AccountId,
                        Currency = "GBP",
                        Name = "Account"
                    },
                    Date = scheduledOn
                })
                .Advance(Duration.FromHours(6));
            
            Assert.False(spec.Has<Account>(AccountId));
            spec.Advance(Duration.FromDays(1))
                .Then(History.AccountCreated);

        }
    }
}