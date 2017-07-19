using System;
using System.Linq;
using Chronos.Core.Accounts.Events;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;
using NodaTime;

namespace Chronos.Core.Accounts.Projections
{

    public class AccountInfoProjector : ProjectorBase<Guid,AccountInfo>
    {
        public AccountInfoProjector(IProjectionWriter writer, IProjectionRepository repository, IEventStoreConnection connection, IEventBus eventBus)
            : base(writer, repository, connection, eventBus) { }

        public AccountInfoProjector WithAccount(Guid accountId)
        {
            var projector = new AccountInfoProjector(Writer, Repository, Connection, EventBus) { Key = accountId };
            
            var projection = Repository.Find<Guid,AccountInfo>(accountId);
            var afterEvent = projection?.LastEvent ?? -1;
            projector.Subscription = new Subscription(StreamExtensions.StreamName<Account>(accountId),afterEvent,projector.When);
            return projector;
        }

        private void When(AccountChanged e,AccountInfo v)
        {
            v.Name = e.Name;
            v.Currency = e.Currency;
        }

        private void When(AccountCreated e, AccountInfo v)
        {
            v.Name = e.Name;
            v.Currency = e.Currency;
            v.Balance = 0;
            v.CreatedAt = e.Timestamp;
        }

        private void When(CashDeposited e, AccountInfo v)
        {
            v.Balance += e.Amount;
        }

        private void When(CashWithdrawn e, AccountInfo v)
        {
            v.Balance -= e.Amount;
        }

        public override void When(IEvent e, AccountInfo v)
        {
            if (e is AccountCreated)
                When(e as AccountCreated,v);
            if (e is AccountChanged)
                When(e as AccountChanged,v);
            if (e is CashDeposited)
                When(e as CashDeposited, v);
            if (e is CashWithdrawn)
                When(e as CashWithdrawn,v);
            v.LastEvent = e.EventNumber;
        }
    }
}