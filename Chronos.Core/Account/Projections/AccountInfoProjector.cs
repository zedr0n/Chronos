﻿using Chronos.Core.Account.Events;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Projections;

namespace Chronos.Core.Account.Projections
{
    public class AccountInfoProjector : Projector<AccountInfo>
        , IConsumer<AccountCreated>
        , IConsumer<AccountChanged>
        , IConsumer<AmountDebited>
    {
        public AccountInfoProjector(IProjectionWriter<AccountInfo> writer, IEventBus eventBus)
            : base(writer, eventBus)
        {
        }

        public void When(AccountChanged e)
        {
            UpdateProjection(e, v =>
            {
                v.Name = e.Name;
                v.Currency = e.Currency;
            });
        }

        public void When(AccountCreated e)
        {
            AddProjection(e, () => new AccountInfo
            {
                Name = e.Name,
                Currency = e.Currency
            });
        }

        public void When(AmountDebited e)
        {
            UpdateProjection(e, v =>
            {
                v.Balance += e.Amount;
            });
        }
    }
}