﻿namespace Chronos.Infrastructure.Queries
{
    public interface IQueryProcessor
    {
        TResult Process<TQuery, TResult>(TQuery query)
            where TQuery : IQuery<TResult>
            where TResult : class, IReadModel, new();
    }
}