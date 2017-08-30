using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;
using Xunit;
using Xunit.Abstractions;

namespace Chronos.Tests
{
    public class ReadTests : ReadTestBase
    {
        public ReadTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CanReadAccountInfo()
        {
            var id = Guid.NewGuid();
            var command = new CreateAccountCommand
            {
                TargetId = id,
                Currency = "GBP",
                Name = "Account"
            };

            var container = CreateContainer(nameof(CanReadAccountInfo));

            var bus = container.GetInstance<ICommandBus>();
            bus.Send(command);

            var query = new AccountInfoQuery {AccountId = id};
            var queryHandler = container.GetInstance<IQueryHandler<AccountInfoQuery, AccountInfo>>();

            var accountInfo = queryHandler.Handle(query);
            Assert.Equal(0,accountInfo.Balance);
        }
    }
}