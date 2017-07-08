using Chronos.Core.Accounts.Events;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Projections;

namespace Chronos.Core.Accounts.Projections
{
    public class AccountInfoProjector : ProjectorBase<AccountInfo>
        , IConsumer<AccountCreated>
        , IConsumer<AccountChanged>
        , IConsumer<AmountDebited>
    {

        public AccountInfoProjector(IEventBus eventBus, IProjectionRepository repository)
            : base(eventBus,repository)
        {
        }

        public void When(AccountChanged e)
        {
            UpdateProjection(e,v =>
            {
                v.Name = e.Name;
                v.Currency = e.Currency;
            }, v => v.AccountId == e.SourceId);
        }

        public void When(AccountCreated e)
        {
            UpdateProjection(e,v =>
            {
                v.AccountId = e.SourceId;
                v.Name = e.Name;
                v.Currency = e.Currency;
                v.Balance = 0;
            }, v => v.AccountId == e.SourceId);
        }

        public void When(AmountDebited e)
        {
            UpdateProjection(e,v =>
            {
                v.Balance += e.Amount;
            },v => v.AccountId == e.SourceId);
        }
    }
}