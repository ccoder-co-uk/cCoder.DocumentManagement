// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.DocumentManagement.Brokers;

internal interface IDmsHttpBroker
{
    DmsProcessingRequest BuildRequest(HttpContext httpContext, App app);
    bool ContainsResponseHeader(HttpContext httpContext, string key);
    void AppendResponseHeader(HttpContext httpContext, string key, string value);
    void SetResponseMetadata(HttpContext httpContext, DmsProcessingResponse dmsProcessingResponse);
    Task CopyResponseBodyAsync(HttpContext httpContext, DmsProcessingResponse dmsProcessingResponse);
    void CloseResponseBody(DmsProcessingResponse dmsProcessingResponse);
}

internal sealed class DmsHttpBroker : IDmsHttpBroker
{
    public DmsProcessingRequest BuildRequest(HttpContext httpContext, App app) =>
        new()
        {
            App = app,
            Method = httpContext.Request.Method,
            RequestPath = httpContext.Request.Path.Value ?? string.Empty,
            Host = httpContext.Request.Host.Host,
            QueryString = httpContext.Request.QueryString.Value ?? string.Empty,
            ContentType = httpContext.Request.Headers.ContentType.ToString(),
            Body = httpContext.Request.Body,
            Headers = httpContext.Request.Headers.ToDictionary(
                keySelector: header => header.Key,
                elementSelector: header => header.Value.ToArray(),
                comparer: StringComparer.OrdinalIgnoreCase),
        };

    public bool ContainsResponseHeader(HttpContext httpContext, string key) =>
        httpContext.Response.Headers.ContainsKey(key: key);

    public void AppendResponseHeader(
        HttpContext httpContext,
        string key,
        string value) =>
        httpContext.Response.Headers.Append(key: key, value: value);

    public void SetResponseMetadata(
        HttpContext httpContext,
        DmsProcessingResponse dmsProcessingResponse)
    {
        httpContext.Response.ContentType = dmsProcessingResponse.ContentType;
        httpContext.Response.StatusCode = dmsProcessingResponse.StatusCode;
    }

    public Task CopyResponseBodyAsync(
        HttpContext httpContext,
        DmsProcessingResponse dmsProcessingResponse) =>
        dmsProcessingResponse.Body.CopyToAsync(destination: httpContext.Response.Body);

    public void CloseResponseBody(DmsProcessingResponse dmsProcessingResponse) =>
        dmsProcessingResponse.Body.Close();
}