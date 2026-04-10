using System.Text.RegularExpressions;
using System;
using System.Text.Json;
using cCoder.Data.Exposures;
using cCoder.DocumentManagement.Exposures.EventHandlers;
using cCoder.DocumentManagement.Exposures.Middleware;
using cCoder.DocumentManagement.Services.Foundations;


namespace cCoder.DocumentManagement;

public static class WebApplicationExtensions
{
    private const string MetadataScope = "DocumentManagement";
    private static readonly Regex DmsRouteRegex = new(@"^\/api\/dms.*", RegexOptions.Compiled);
    private static readonly Regex WebDavRouteRegex = new(@"^\/api\/webdav.*", RegexOptions.Compiled);

    public static WebApplication UseDocumentManagementExposure(
        this WebApplication app,
        ILogger log = null
    )
    {
        log?.LogInformation("Initialising Document Management");
        PopulateMetadataTypeCache(app);
        app.MapWhen(
            context => DmsRouteRegex.IsMatch(context.Request.Path.Value?.ToLower() ?? string.Empty),
            branch => branch.UseMiddleware<DMSMiddleware>()
        );

        app.MapWhen(
            context => WebDavRouteRegex.IsMatch(context.Request.Path.Value?.ToLower() ?? string.Empty),
            branch => branch.UseMiddleware<WebDavMiddleware>()
        );

        return app;
    }

    public static WebApplication UseDocumentManagementEventHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        foreach (IDocumentManagementEventHandlers handlers in services.GetServices<IDocumentManagementEventHandlers>())
            handlers.ListenToAllEvents();

        return app;
    }

    private static void PopulateMetadataTypeCache(WebApplication app)
    {
        IMetadataTypeCache metadataTypeCache = app.Services.GetRequiredService<IMetadataTypeCache>();

        if (!metadataTypeCache.Contains(MetadataScope))
        {
            metadataTypeCache.Set(
                MetadataScope,
                app.Services
                    .GetRequiredService<IDocumentManagementMetadataTypeService>()
                    .GetKnownMetadata()
                    .Select(static metadata => JsonSerializer.Serialize(metadata)));
        }
    }
}




