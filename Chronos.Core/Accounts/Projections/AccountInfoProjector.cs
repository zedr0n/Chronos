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

    public class AccountInfoProjector : IProjector<Guid,AccountInfo>
    {
        public IProjectionWriter Writer { get; private set; }
        public IProjectionRepository Repository { get; private set; }
        private readonly IEventBus _eventBus;
        public IEventStoreConnection Connection { get; private set; }

        public AccountInfoProjector(IProjectionWriter writer, IEventStoreConnection connection, IProjectionRepository repository, IEventBus eventBus)
        {
            Writer = writer;
            Connection = connection;
            Repository = repository;
            _eventBus = eventBus;
        }

        public Guid Key { get; private set; }

        public Subscription Subscription { get; private set; }

        public void Start()
        {
            Connection.SubscribeToStream(Subscription.StreamName,Subscription.EventNumber,Subscription.OnEvent);
        }

        public void Reset()
        {
            Connection.DropSubscription(Subscription.StreamName, Subscription.OnEvent);
            Start();
        }

        public AccountInfoProjector WithAccount(Guid accountId)
        {
            var projector = new AccountInfoProjector(Writer, Connection, Repository, _eventBus) { Key = accountId };
            _eventBus.Subscribe<ReplayCompleted>(e => projector.Reset());
            
            var projection = Repository.Find<Guid,AccountInfo>(accountId);
            if(projection == null)
                Repository.Add(new AccountInfo { Key = accountId });
            var afterEvent = projection?.LastEvent ?? -1;
            projector.Subscription = new Subscription(StreamExtensions.StreamName<Account>(accountId),afterEvent,projector.When);
            return projector;
        }

        public void When(AccountChanged e,AccountInfo v)
        {
            v.Name = e.Name;
            v.Currency = e.Currency;
        }
        public void When(AccountCreated e, AccountInfo v)
        {
            v.Name = e.Name;
            v.Currency = e.Currency;
            v.Balance = 0;
            v.CreatedAt = e.Timestamp;
        }
        public void When(CashDeposited e, AccountInfo v)
        {
            v.Balance += e.Amount;
        }
        public void When(CashWithdrawn e, AccountInfo v)
        {
            v.Balance -= e.Amount;
        }

        public void When(IEvent e, AccountInfo v)
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



        public void When(IEvent e)
        {
            Writer.UpdateOrThrow<Guid,AccountInfo>(Key, v => When(e,v));
        }
    }
}