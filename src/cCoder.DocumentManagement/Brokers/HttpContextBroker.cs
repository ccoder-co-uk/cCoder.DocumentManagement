// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Brokers;

internal interface IHttpContextBroker
{
    string GetRequestPath();

    string GetRequestHost();
}

internal sealed class HttpContextBroker(HttpContext httpContext) : IHttpContextBroker
{
    public string GetRequestPath() =>
        httpContext?.Request.Path.Value ?? string.Empty;

    public string GetRequestHost() =>
        httpContext?.Request.Host.Host ?? string.Empty;
}