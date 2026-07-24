// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace cCoder.DocumentManagement.Dependencies;

internal static class Branching
{
    internal static IQueryable<T> ApplyQueryFilters<T>(
        IQueryable<T> query,
        bool ignoreFilters)
        where T : class =>
        ignoreFilters
            ? query.IgnoreQueryFilters()
            : query;

    internal static void ThrowWhen(
        bool condition,
        Func<Exception> createException)
    {
        if (condition)
        {
            throw createException();
        }
    }

    internal static TOutput MapOrDefault<TInput, TOutput>(
        TInput input,
        Func<TInput, TOutput> mapper)
        where TInput : class
        where TOutput : class =>
        input is null
            ? null
            : mapper(input);

    internal static async ValueTask<TResult> ExecuteWhenNotNullAsync<TInput, TResult>(
        TInput input,
        Func<TInput, ValueTask<TResult>> operation,
        TResult defaultValue)
        where TInput : class =>
        input is null
            ? defaultValue
            : await operation(input);
}
