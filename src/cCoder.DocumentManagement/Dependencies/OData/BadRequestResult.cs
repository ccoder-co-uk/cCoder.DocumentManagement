// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace cCoder.DocumentManagement.Models.OData;

public sealed class BadRequestResult : BadRequestObjectResult
{
    public BadRequestResult(ModelStateDictionary modelState)
        : base(modelState: modelState) =>
        Value = SerializeForOData(
            value: modelState
                .Select(selector: item => new ModelStateError
                {
                    Key = item.Key,
                    Value = item.Value?.RawValue,
                    Errors = item.Value?.Errors?
                        .Select(selector: error =>
                            $"{error.ErrorMessage} - {error.Exception?.Message}")
                        .ToArray(),
                })
                .ToArray());

    private static string SerializeForOData(object value) =>
        JsonConvert.SerializeObject(
            value: value,
            formatting: Formatting.None,
            settings: new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true
                },
                MaxDepth = 4,
            });
}