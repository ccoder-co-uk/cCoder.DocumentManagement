// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;
using System;
using System.Text.Json;
using cCoder.Data.Exposures;
using cCoder.DocumentManagement.Exposures.EventHandlers;
using cCoder.DocumentManagement.Exposures.Middleware;
using cCoder.DocumentManagement.Services.Foundations;


namespace cCoder.DocumentManagement;

public static partial class WebApplicationExtensions
{
    private const string MetadataScope = "DocumentManagement";
    private static readonly Regex DmsRouteRegex = new(pattern: @"^\/api\/dms.*", options: RegexOptions.Compiled);
    private static readonly Regex WebDavRouteRegex = new(pattern: @"^\/api\/webdav.*", options: RegexOptions.Compiled);

    public static WebApplication StartDocumentManagementWeb(
        this WebApplication app,
        ILogger log = null) =>
        app.UseDocumentManagementExposure(log: log)
            .UseDocumentManagementEventHandlers();

    public static WebApplication StartDocumentManagementHostedServices(this WebApplication app) =>
        app.UseDocumentManagementEventHandlers();

    private static WebApplication UseDocumentManagementExposure(
        this WebApplication app,
        ILogger log = null
    )
    {
        log?.LogInformation(message: "Initialising Document Management");
        PopulateMetadataTypeCache(app: app);
        app.MapWhen(
            predicate: context => DmsRouteRegex.IsMatch(context.Request.Path.Value?.ToLower() ?? string.Empty),
            configuration: branch => branch.UseMiddleware<DMSMiddleware>()
        );

        app.MapWhen(
            predicate: context => WebDavRouteRegex.IsMatch(context.Request.Path.Value?.ToLower() ?? string.Empty),
            configuration: branch => branch.UseMiddleware<WebDavMiddleware>()
        );

        return app;
    }

    private static WebApplication UseDocumentManagementEventHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        foreach (IDocumentManagementEventHandlers handlers in services.GetServices<IDocumentManagementEventHandlers>())
        {
            handlers.ListenToAllEvents();
        }

        return app;
    }

    private static void PopulateMetadataTypeCache(WebApplication app)
    {
        IMetadataTypeCache metadataTypeCache = app.Services.GetRequiredService<IMetadataTypeCache>();

        if (!metadataTypeCache.Contains(scope: MetadataScope))
        {
            metadataTypeCache.Set(
                scope: MetadataScope,
                typeSetPayloads: app.Services
                    .GetRequiredService<IDocumentManagementMetadataTypeService>()
                    .GetKnownMetadata()
                    .Select(static metadata => JsonSerializer.Serialize(metadata)));
        }
    }
}