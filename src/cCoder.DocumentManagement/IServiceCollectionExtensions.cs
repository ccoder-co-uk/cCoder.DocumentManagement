using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Packaging;
using cCoder.DocumentManagement.Api.OData;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Brokers.Events;
using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Exposures.EventHandlers;
using cCoder.DocumentManagement.Services;
using cCoder.DocumentManagement.Services.Aggregations;
using cCoder.DocumentManagement.Services.Coordinations;
using cCoder.DocumentManagement.Services.Foundations;
using cCoder.DocumentManagement.Services.Foundations.Events;
using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.DocumentManagement.Services.Processings;
using cCoder.Eventing;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;
using AuthorizationBroker = cCoder.DocumentManagement.Brokers.AuthorizationBroker;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;
using IJsonBroker = cCoder.DocumentManagement.Brokers.IJsonBroker;
using IRoleBroker = cCoder.DocumentManagement.Brokers.IRoleBroker;
using JsonBroker = cCoder.DocumentManagement.Brokers.JsonBroker;
using RoleBroker = cCoder.DocumentManagement.Brokers.RoleBroker;


namespace cCoder.DocumentManagement;

public static class IServiceCollectionExtensions
{
    public static void AddDocumentManagement(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddEventHandlers();
    }

    public static void AddDocumentManagementApi(this IServiceCollection services, ODataConventionModelBuilder builder = null)
    {
        services.AddDocumentManagement();
        services.AddApi("DocumentManagement", ConfigureDocumentManagementApiModel, builder);
    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<App>();
        services.AddEventingForType<FileContent>();
        services.AddEventingForType<Package>();
        services.AddEventingForType<PackageItem>();
        services.AddEventingForType<(int, Package)>();
        services.AddEventingForType<cCoder.Data.Models.DMS.File>();
        services.AddEventingForType<Folder>();
        services.AddEventingForType<cCoder.Data.Models.Security.FolderRole>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<IEventHubBroker, EventHubBroker>();
        services.AddTransient<IDmsInstanceFactory, DmsInstanceFactory>();
        services.AddTransient<IDmsInstanceBroker, DmsInstanceBroker>();
        services.AddTransient<IFileContentEventBroker, FileContentEventBroker>();
        services.AddTransient<IFileEventBroker, FileEventBroker>();
        services.AddTransient<IFolderEventBroker, FolderEventBroker>();
        services.AddTransient<IFolderRoleEventBroker, FolderRoleEventBroker>();
        services.AddTransient<IFileBroker, FileBroker>();
        services.AddTransient<IFileContentBroker, FileContentBroker>();
        services.AddTransient<IFolderBroker, FolderBroker>();
        services.AddTransient<IFolderRoleBroker, FolderRoleBroker>();
        services.AddTransient<IAppBroker, AppBroker>();
        services.AddTransient<IRoleBroker, RoleBroker>();
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
        services.AddTransient<IJsonBroker, JsonBroker>();
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<IDocumentManagementAppExposure, DocumentManagementAppExposure>();
        services.AddTransient<IDocumentManagementPackageManager, DocumentManagementPackageManager>();
        services.AddTransient<IDocumentManagementEventHandlers, DocumentManagementEventHandlers>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<Services.Foundations.Events.IEventHandlerService, Services.Foundations.Events.EventHandlerService>();
        services.AddTransient<IDmsInstanceService, DmsInstanceService>();
        services.AddTransient<IFileContentService, FileContentService>();
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<IDocumentManagementMetadataTypeService, DocumentManagementMetadataTypeService>();
        services.AddTransient<IFolderRoleService, FolderRoleService>();
        services.AddTransient<IFolderService, FolderService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IFileContentEventService, FileContentEventService>();
        services.AddTransient<IFileEventService, FileEventService>();
        services.AddTransient<IFolderEventService, FolderEventService>();
        services.AddTransient<IFolderRoleEventService, FolderRoleEventService>();
        services.AddTransient<IDocumentManagementCurrentAppResolver, CurrentAppResolver>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<IAppOrchestrationService, AppOrchestrationService>();
        services.AddTransient<IDocumentManagementMigrationAggregationService, DocumentManagementMigrationAggregationService>();
        services.AddTransient<IDmsOrchestrationService, DmsOrchestrationService>();
        services.AddTransient<IDmsHttpRequestOrchestrationService, DmsHttpRequestOrchestrationService>();
        services.AddTransient<IFileContentOrchestrationService, FileContentOrchestrationService>();
        services.AddTransient<IFileOrchestrationService, FileOrchestrationService>();
        services.AddTransient<IFolderOrchestrationService, FolderOrchestrationService>();
        services.AddTransient<IFolderRoleOrchestrationService, FolderRoleOrchestrationService>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<IFolderCoordinationService, FolderCoordinationService>();
        services.AddTransient<IDmsProcessingService, DmsProcessingService>();
        services.AddTransient<IFileContentEventProcessingService, FileContentEventProcessingService>();
        services.AddTransient<IFileContentProcessingService, FileContentProcessingService>();
        services.AddTransient<IFileEventProcessingService, FileEventProcessingService>();
        services.AddTransient<IFileProcessingService, FileProcessingService>();
        services.AddTransient<IFolderEventProcessingService, FolderEventProcessingService>();
        services.AddTransient<IFolderProcessingService, FolderProcessingService>();
        services.AddTransient<IFolderRoleEventProcessingService, FolderRoleEventProcessingService>();
        services.AddTransient<IFolderRoleProcessingService, FolderRoleProcessingService>();
        services.AddTransient<IWebDavProcessingService, WebDavProcessingService>();
    }

    private static void ConfigureDocumentManagementApiModel(ODataConventionModelBuilder builder) =>
        new DocumentManagementModelBuilder(builder).Configure();

    private static void AddApi(
        this IServiceCollection services,
        string routePrefix,
        Action<ODataConventionModelBuilder> configureModel,
        ODataConventionModelBuilder builder = null,
        bool useFullSchemaIds = false)
    {
        services.AddSingleton<Action<ODataConventionModelBuilder>>(configureModel);

        if (builder is not null)
            configureModel(builder);

        AddAspNet(services);

        if (builder is null)
            AddApiDocumentation(services, routePrefix, useFullSchemaIds);

        IEdmModel routeModel = BuildRouteModel(configureModel);
        DefaultODataBatchHandler batchHandler = new();

        services.AddControllers().AddOData(options =>
        {
            options.RouteOptions.EnableQualifiedOperationCall = false;
            options.EnableAttributeRouting = true;
            options.RouteOptions.EnableKeyAsSegment = false;
            options.Expand()
                .Count()
                .Filter()
                .Select()
                .OrderBy()
                .SetMaxTop(1000)
                .AddRouteComponents($"Api/{routePrefix}", routeModel, batchHandler);

            if (builder is null)
                _ = options.AddRouteComponents("Api/Core", routeModel, batchHandler);
        });
    }

    private static void AddApiDocumentation(
        IServiceCollection services,
        string routePrefix,
        bool useFullSchemaIds)
    {
        services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            AddSwaggerDocuments(options, routePrefix);
            options.DocInclusionPredicate(
                (documentName, apiDescription) =>
                    ShouldIncludeInDocument(documentName, apiDescription.RelativePath, routePrefix));

            if (useFullSchemaIds)
                options.CustomSchemaIds(type => type.FullName?.Replace('+', '.') ?? type.Name);

            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Description = @"Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
            });
        });
    }

    private static void AddSwaggerDocuments(
        Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options,
        string routePrefix)
    {
        options.SwaggerDoc(routePrefix, new OpenApiInfo
        {
            Title = $"{routePrefix} API definition",
            Version = routePrefix,
        });
        options.SwaggerDoc("Core", new OpenApiInfo
        {
            Title = "Core API definition",
            Version = "Core",
        });
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Core API definition",
            Version = "v1",
        });
    }

    private static bool ShouldIncludeInDocument(
        string documentName,
        string relativePath,
        string routePrefix)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        if (string.Equals(documentName, "v1", StringComparison.OrdinalIgnoreCase))
            documentName = "Core";

        string path = NormalizePath(relativePath);

        return string.Equals(documentName, "Core", StringComparison.OrdinalIgnoreCase)
            ? MatchesContextRoute(path, "Core")
            : MatchesContextRoute(path, routePrefix);
    }

    private static bool MatchesContextRoute(string path, string context)
    {
        string prefix = $"/Api/{context}";
        return path.Equals(prefix, StringComparison.OrdinalIgnoreCase)
            || path.StartsWith($"{prefix}/", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string relativePath) =>
        relativePath.StartsWith('/') ? relativePath : $"/{relativePath}";

    private static IEdmModel BuildRouteModel(Action<ODataConventionModelBuilder> configureModel)
    {
        ODataConventionModelBuilder builder = new();
        configureModel(builder);
        return builder.GetEdmModel();
    }

    private static void AddAspNet(IServiceCollection services)
    {
        services.AddRouting();
        services.AddResponseCompression();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddScoped(
            typeof(HttpContext),
            ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext ?? new DefaultHttpContext());
        services.AddScoped(typeof(HttpRequest), ctx => ctx.GetRequiredService<HttpContext>().Request);
        services.AddSession();
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromMinutes(60);
        });
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddRazorPages();
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });
        services.AddEndpointsApiExplorer();
        services.AddSignalR();
    }
}










