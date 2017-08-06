﻿namespace Chronos.Infrastructure.Projections.New
{
    public interface IProjectionManager
    {
        IBaseProjectionExpression<T> Create<T>()
            where T : class, IReadModel, new();
    }
}