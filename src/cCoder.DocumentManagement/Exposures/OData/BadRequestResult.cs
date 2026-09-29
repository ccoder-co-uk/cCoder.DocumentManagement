// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.DocumentManagement.Extensions.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace cCoder.DocumentManagement.Exposures.OData;

public sealed class BadRequestResult : BadRequestObjectResult
{
    public BadRequestResult(ModelStateDictionary modelState)
        : base(error: modelState
            .Select(selector: item => new ModelStateError
            {
                Key = item.Key,
                Value = item.Value?.RawValue,
                Errors = item.Value?.Errors?
                    .Select(selector: error =>
                        $"{error.ErrorMessage} - {error.Exception?.Message}")
                    .ToArray(),
            })
            .ToArray())
    {
    }
}