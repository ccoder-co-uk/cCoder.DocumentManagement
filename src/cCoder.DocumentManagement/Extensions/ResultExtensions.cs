// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Extensions;

public static class ResultExtensions
{
    public static Result<TNew> ToNew<T, TNew>(
        this Result<T> result,
        TNew item) =>
        new()
        {
            Success = result.Success,
            Message = result.Message,
            Item = item
        };
}