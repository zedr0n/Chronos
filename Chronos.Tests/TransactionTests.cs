using System;
using Chronos.Core.Accounts;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Transactions;
using Chronos.Core.Transactions.Commands;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Projections;
using Chronos.Persistence;
using Xunit;

namespace Chronos.Tests
{
    public class TransactionTests : TestsBase
    {
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

            var projectionRepository = container.GetInstance<IRepository<AccountInfo>>();
            var accountInfo = projectionRepository.Get(accountId);

            Assert.NotNull(purchase);
            Assert.Equal(100, accountInfo.Balance);
        }
    }
}