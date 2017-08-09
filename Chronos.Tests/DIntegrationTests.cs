using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Chronos.Core.Accounts;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Events;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
using Chronos.Core.Transactions.Commands;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Queries;
using Xunit;
using Xunit.Abstractions;

namespace Chronos.Tests
{

    public class DIntegrationTests : DTestBase
    {
        public DIntegrationTests(ITestOutputHelper output) : base(output)
        {
            
        }

        private class DCanCreateAccount
        {
            private readonly ICommandHandler<CreateAccountCommand> _commandHandler;
            private readonly IDomainRepository _domainRepository;

            public DCanCreateAccount(ICommandHandler<CreateAccountCommand> commandHandler, IDomainRepository domainRepository)
            {
                _commandHandler = commandHandler;
                _domainRepository = domainRepository;
            }

            public void Test()
            {
                var id = Guid.NewGuid();
                var command = new CreateAccountCommand
                {
                    TargetId = id,
                    Currency = "GBP",
                    Name = "Account"
                };
                
                _commandHandler.Handle(command);
                _domainRepository.Get<Account>(id);
            }
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
        public void CanCreateAccountEx() 
            => GetInstance<DCanCreateAccount>().Test();

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
                    Amount = 100,
                    Payee = "Payee"
                })
                .Then(History.PurchaseCreated, History.CashWithdrawn);
        }
    }
}