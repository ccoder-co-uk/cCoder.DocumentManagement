// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;

namespace cCoder.DocumentManagement.Extensions;

internal static class IQueryableExtensions
{
    internal static IQueryable<T> ApplyQueryFilters<T>(
        this IQueryable<T> query,
        bool ignoreFilters)
        where T : class =>
        new QueryFilterBroker().ApplyQueryFilters(
            query: query,
            ignoreFilters: ignoreFilters);
}