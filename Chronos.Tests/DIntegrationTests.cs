using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Chronos.Core.Accounts;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Events;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
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
        }
        
        [Fact]
        public void CanCreateAccountEx() 
            => GetInstance<DCanCreateAccount>().Test();

        [Fact]
        public void CanCreateAccount()
        { 
            GetInstance<Bdd>().When(
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
            var test = GetInstance<Bdd>();
            test.Given<Account>(History.AccountId, History.AccountCreated)
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
            var test = GetInstance<Bdd>();
            test.When(new CreateAccountCommand
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
            var test = GetInstance<Bdd>();
            var accountInfo = test.Given<Account>(History.AccountId, History.AccountCreated)
                .Query<AccountInfoQuery,AccountInfo>(new AccountInfoQuery
                {
                    AccountId = History.AccountId
                });
            Assert.Equal("Account",accountInfo.Name);
            Assert.Equal("GBP",accountInfo.Currency);
        }
    }
}