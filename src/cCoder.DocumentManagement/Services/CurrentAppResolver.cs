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
            requestPath.Contains("/webdav", StringComparison.OrdinalIgnoreCase)
            && requestPath.Contains("Core/App(", StringComparison.OrdinalIgnoreCase)
        )
        {
            int start = requestPath.IndexOf("Core/App(", StringComparison.OrdinalIgnoreCase) + 9;
            int end = requestPath.IndexOf(')', start);

            if (end > start && int.TryParse(requestPath[start..end], out int appId))
                return ToResolvedApp(appBroker.GetAppById(appId))
                    ?? throw new InvalidOperationException($"Unable to resolve app '{appId}'.");
        }

        string host = httpContext?.Request.Host.Host ?? string.Empty;
        return ToResolvedApp(appBroker.GetAppByDomain(host))
            ?? throw new InvalidOperationException($"Unable to resolve current app for host '{host}'.");
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



