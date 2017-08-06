using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Transactions.Commands;
using Chronos.Infrastructure.Commands;
using Xunit;
using Xunit.Abstractions;

namespace Chronos.Tests
{
    public class PerformanceTests : TestBase
    {   
        [Theory]
        [InlineData(200)]
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
        }

        public PerformanceTests(ITestOutputHelper output) : base(output)
        {
            CanAddMultipleTransactionsQuickly(1);
        }
    }
}