// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace cCoder.DocumentManagement.Extensions;

internal static class QueryFilterExtensions
{
    internal static IQueryable<T> ApplyQueryFilters<T>(
        this IQueryable<T> query,
        bool ignoreFilters)
        where T : class =>
        ignoreFilters
            ? query.IgnoreQueryFilters()
            : query;
}