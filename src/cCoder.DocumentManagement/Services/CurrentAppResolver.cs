// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.DocumentManagement.Brokers.Storage;


namespace cCoder.DocumentManagement.Services;

internal class CurrentAppResolver(
    IAppBroker appBroker,
    HttpContext httpContext = null
) : IDocumentManagementCurrentAppResolver
{
    public App ResolveCurrentApp()
    {
        string requestPath = httpContext?.Request.Path.Value ?? string.Empty;

        if (
            requestPath.Contains(value: "/webdav", comparisonType: StringComparison.OrdinalIgnoreCase)
            && requestPath.Contains(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase)
        )
        {
            int start = requestPath.IndexOf(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase) + 9;
            int end = requestPath.IndexOf(value: ')', startIndex: start);

            if (end > start && int.TryParse(s: requestPath[start..end], result: out int appId))
            {
                return ToResolvedApp(app: appBroker.GetAppById(appId: appId))
                    ?? throw new InvalidOperationException(message: $"Unable to resolve app '{appId}'.");
            }
        }

        string host = httpContext?.Request.Host.Host ?? string.Empty;

        return ToResolvedApp(app: appBroker.GetAppByDomain(domain: host))
            ?? throw new InvalidOperationException(message: $"Unable to resolve current app for host '{host}'.");
    }

    private static App ToResolvedApp(App app) =>
        app == null
            ? null
            : new App
            {
                Id = app.Id,
                DefaultCultureId = app.DefaultCultureId,
                TenantId = app.TenantId,
                Name = app.Name,
                Domain = app.Domain,
                DefaultTheme = app.DefaultTheme,
                ConfigJson = app.ConfigJson,
                Roles = [],
                Folders = [],
            };
}