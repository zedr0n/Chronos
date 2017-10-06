using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
using Chronos.Core.Transactions.Commands;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;
using Xunit;
using Xunit.Abstractions;

namespace Chronos.Tests
{
    public class PerformanceTests : TestBase
    {   
        [Theory]
        [InlineData(1000)]
        public void CanAddMultipleTransactionsQuickly(int numberOfTransactions)
        {
            var container = CreateContainer(nameof(CanAddMultipleTransactionsQuickly));

            var accountId = Guid.NewGuid();
            var createAccountCommand = new CreateAccountCommand
            {
                TargetId = accountId,
                Currency = "GBP",
                Name = "Account"
            };

            var handler = container.GetInstance<ICommandHandler<CreateAccountCommand>>();
            handler.Handle(createAccountCommand);

            var handler2 = container.GetInstance<ICommandHandler<CreatePurchaseCommand>>();

            var totalMovement = 100 * numberOfTransactions;
            
            while (numberOfTransactions-- > 0)
            {
                var command = new CreatePurchaseCommand
                {
                    TargetId = Guid.NewGuid(),
                    AccountId = accountId,
                    Amount = 100,
                    Currency = "GBP",
                    Payee = "Payee"
                };

                handler2.Handle(command);
            }

            var processor = container.GetInstance<IQueryProcessor>();

            var movement = processor.Process<TotalMovementQuery,TotalMovement>(new TotalMovementQuery());
            Assert.Equal(totalMovement,movement.Value);
        }

        public PerformanceTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}