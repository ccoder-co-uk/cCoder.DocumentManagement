// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;

namespace cCoder.DocumentManagement;

public static partial class IServiceCollectionExtensions
{
    internal static void ConfigureDocumentManagementApi(
        this IServiceCollection services,
        Models.DocumentManagementConfiguration configuration,
        Action<ODataConventionModelBuilder> configureModel,
        bool includeDocumentation)
    {
        services.AddDocumentManagementAspNet();

        if (includeDocumentation)
        {
            services.AddDocumentManagementApiDocumentation(
                configuration: configuration);
        }

        IEdmModel routeModel = services.BuildDocumentManagementRouteModel(
            configureModel: configureModel);
        DefaultODataBatchHandler batchHandler = new();
        string rootPath = string.IsNullOrWhiteSpace(value: configuration.RootPath)
            ? "Api/DocumentManagement"
            : configuration.RootPath;

        IMvcBuilder mvcBuilder = services.AddControllers();
        mvcBuilder.AddOData(setupAction: options =>
        {
            options.RouteOptions.EnableQualifiedOperationCall = false;
            options.EnableAttributeRouting = true;
            options.RouteOptions.EnableKeyAsSegment = false;
            options.Expand()
                .Count()
                .Filter()
                .Select()
                .OrderBy()
                .SetMaxTop(maxTopValue: 1000)
                .AddRouteComponents(
                    routePrefix: rootPath,
                    model: routeModel,
                    batchHandler: batchHandler);
        });
    }

    private static void AddDocumentManagementApiDocumentation(
        this IServiceCollection services,
        Models.DocumentManagementConfiguration configuration) =>
        services.AddSwaggerGen(setupAction: options =>
        {
            options.ResolveConflictingActions(
                resolver: apiDescriptions => apiDescriptions.First());
            options.SwaggerDoc(
                name: "DocumentManagement",
                info: new OpenApiInfo
                {
                    Title = "DocumentManagement API definition",
                    Version = "DocumentManagement",
                });
            options.DocInclusionPredicate(
                predicate: (documentName, apiDescription) =>
                    services.ShouldIncludeInDocument(
                        documentName: documentName,
                        relativePath: apiDescription.RelativePath,
                        configuration: configuration));
            options.AddSecurityDefinition(
                name: "bearer",
                securityScheme: new OpenApiSecurityScheme
                {
                    Description = "Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                });
        });

    private static bool ShouldIncludeInDocument(
        this IServiceCollection services,
        string documentName,
        string relativePath,
        Models.DocumentManagementConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(value: relativePath))
        {
            return false;
        }

        string path = services.NormalizeDocumentManagementPath(
            relativePath: relativePath);
        string rootPath = string.IsNullOrWhiteSpace(value: configuration.RootPath)
            ? "Api/DocumentManagement"
            : configuration.RootPath;
        string normalizedRootPath = services.NormalizeDocumentManagementPath(
            relativePath: rootPath);

        return string.Equals(
                a: documentName,
                b: "DocumentManagement",
                comparisonType: StringComparison.OrdinalIgnoreCase)
            && (path.Equals(
                    value: normalizedRootPath,
                    comparisonType: StringComparison.OrdinalIgnoreCase)
                || path.StartsWith(
                    value: $"{normalizedRootPath}/",
                    comparisonType: StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeDocumentManagementPath(
        this IServiceCollection services,
        string relativePath) =>
        relativePath.StartsWith(value: '/') ? relativePath : $"/{relativePath}";

    private static IEdmModel BuildDocumentManagementRouteModel(
        this IServiceCollection services,
        Action<ODataConventionModelBuilder> configureModel)
    {
        ODataConventionModelBuilder builder = new();
        configureModel(obj: builder);
        return builder.GetEdmModel();
    }

    private static void AddDocumentManagementAspNet(
        this IServiceCollection services)
    {
        services.AddRouting();
        services.AddResponseCompression();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddScoped(
            serviceType: typeof(HttpContext),
            implementationFactory: context =>
                context.GetService<IHttpContextAccessor>()?.HttpContext
                    ?? new DefaultHttpContext());
        services.AddScoped(
            serviceType: typeof(HttpRequest),
            implementationFactory: context =>
                context.GetRequiredService<HttpContext>().Request);
        services.AddSession();
        services.AddHsts(configureOptions: options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromMinutes(minutes: 60);
        });
        services.AddMvc(
            setupAction: options => options.EnableEndpointRouting = false);
        services.AddRazorPages();
        services.Configure<KestrelServerOptions>(configureOptions: options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });
        services.AddEndpointsApiExplorer();
        services.AddSignalR();
    }
}