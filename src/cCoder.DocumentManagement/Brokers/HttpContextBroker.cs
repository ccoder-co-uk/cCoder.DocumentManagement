// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;

namespace cCoder.DocumentManagement.Brokers;

internal interface IHttpContextBroker
{
    string GetRequestPath();

    string GetRequestHost();
}

internal sealed class HttpContextBroker(HttpContext httpContext)
    : IHttpContextBroker, IUtilityBroker
{
    public string GetRequestPath() =>
        httpContext?.Request.Path.Value ?? string.Empty;

    public string GetRequestHost() =>
        httpContext?.Request.Host.Host ?? string.Empty;
}