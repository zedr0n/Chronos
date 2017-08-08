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
        
        [Fact]
        public void CanCreateAccount() 
            => GetInstance<DCanCreateAccount>().Test();

        [Fact]
        public void CanCreateAccountEx()
        {
            var accountId = Guid.NewGuid();
            GetInstance<Bdd>().When(
                new CreateAccountCommand
                {
                    TargetId = accountId,
                    Currency = "GBP",
                    Name = "Account"
                }).Then(events => events.OfType<AccountCreated>().All(e =>
                    e.AccountId == accountId && e.Currency == "GBP" && e.Name == "Account" ));
        }

        private class DCanCreateMultipleAccounts
        {
            private readonly IQueryHandler<AccountInfoQuery, AccountInfo> _queryHandler;
            private readonly ICommandHandler<CreateAccountCommand> _createHandler;

            public DCanCreateMultipleAccounts(ICommandHandler<CreateAccountCommand> createHandler, 
                IQueryHandler<AccountInfoQuery, AccountInfo> queryHandler)
            {
                _createHandler = createHandler;
                _queryHandler = queryHandler;
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

                _createHandler.Handle(command);

                var otherId = Guid.NewGuid();
                var otherCommand = new CreateAccountCommand
                {
                    TargetId = otherId,
                    Currency = "GBP",
                    Name = "OtherAccount"
                };

                _createHandler.Handle(otherCommand);

                var query = new AccountInfoQuery { AccountId = id };
                var otherQuery = new AccountInfoQuery {AccountId = otherId };

                var accountInfo = _queryHandler.Handle(query);
                Assert.Equal("Account",accountInfo.Name);
                var otherAccountInfo = _queryHandler.Handle(otherQuery);
                Assert.Equal("OtherAccount",otherAccountInfo.Name);
            }
        }

        [Fact]
        public void CanCreateMultipleAccounts()
            => GetInstance<DCanCreateMultipleAccounts>().Test();
    }
}