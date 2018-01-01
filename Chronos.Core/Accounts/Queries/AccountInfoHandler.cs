using System;
using System.Collections.Generic;
using System.ComponentModel;
using Chronos.Core.Accounts.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;
using NodaTime;

namespace Chronos.Core.Accounts.Queries
{
    public class AccountInfoHandler : IQueryHandler<AccountInfoQuery, AccountInfo>
    {
        private readonly IReadRepository _repository;
        public IProjectionExpression<AccountInfo> Expression { get; }

        public AccountInfoHandler(IReadRepository repository, IProjectionManager manager)
        {
            _repository = repository;

            Expression = manager.Create<AccountInfo>()
                .From<Account>()
                .ForEachStream()
                .OutputState();
                
             Expression.Invoke();
        }

        public AccountInfo Handle(AccountInfoQuery query)
        {
            var accountInfo = _repository.Find<Guid, AccountInfo>(query.AccountId);
            return accountInfo;
        }
    }
}