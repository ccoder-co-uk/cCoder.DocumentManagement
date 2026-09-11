// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace cCoder.DocumentManagement.Brokers;

internal interface IQueryFilterBroker
{
    IQueryable<T> ApplyQueryFilters<T>(IQueryable<T> query, bool ignoreFilters)
        where T : class;
}

internal sealed class QueryFilterBroker : IQueryFilterBroker
{
    public IQueryable<T> ApplyQueryFilters<T>(
        IQueryable<T> query,
        bool ignoreFilters)
        where T : class
    {
        Func<IQueryable<T>>[] querySelectors =
        [
            () => query,
            () => query.IgnoreQueryFilters(),
        ];

        return querySelectors[Convert.ToInt32(value: ignoreFilters)]();
    }
}