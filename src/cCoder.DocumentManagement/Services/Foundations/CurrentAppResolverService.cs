// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Brokers.Storage;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class CurrentAppResolverService(
    IAppBroker appBroker,
    IHttpContextBroker httpContextBroker)
    : ICurrentAppResolverService
{
    public App ResolveCurrentApp() =>
        TryCatch(operation: () =>
        {
            string requestPath = httpContextBroker.GetRequestPath();

            if (
                requestPath.Contains(value: "/webdav", comparisonType: StringComparison.OrdinalIgnoreCase)
                && requestPath.Contains(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                int start = requestPath.IndexOf(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase) + 9;
                int end = requestPath.IndexOf(value: ')', startIndex: start);

                if (end > start && int.TryParse(s: requestPath[start..end], result: out int appId))
                {
                    return appBroker.SelectAppById(appId: appId)
                        ?? throw new InvalidOperationException(message: $"Unable to resolve app '{appId}'.");
                }
            }

            string host = httpContextBroker.GetRequestHost();

            return appBroker.SelectAppByDomain(domain: host)
                ?? throw new InvalidOperationException(message: $"Unable to resolve current app for host '{host}'.");
        });
}