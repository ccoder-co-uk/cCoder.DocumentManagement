// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Packaging;
using cCoder.DocumentManagement.Extensions;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Brokers.OData;
using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Exposures.Middleware;
using cCoder.DocumentManagement.Brokers.Events;
using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Dependencies;
using cCoder.DocumentManagement.Extensions.OData;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Services;
using cCoder.DocumentManagement.Services.Aggregations;
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

public static partial class IServiceCollectionExtensions
{
    public static void AddDocumentManagementWeb(
        this IServiceCollection services,
        Action<DocumentManagementConfiguration> configure = null,
        ODataConventionModelBuilder builder = null)
    {
        DocumentManagementConfiguration configuration = DocumentManagementConfigurationFactory.CreateDocumentManagementConfiguration();
        configure?.Invoke(obj: configuration);
        services.AddDocumentManagementWeb(configuration: configuration, builder: builder);
    }

    public static void AddDocumentManagementWeb(
        this IServiceCollection services,
        DocumentManagementConfiguration configuration,
        ODataConventionModelBuilder builder = null)
    {
        services.RegisterDocumentManagementConfiguration(
            configuration: configuration);
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddEventHandlers();
        services.AddTransient<DMSMiddleware>();
        services.AddTransient<WebDavMiddleware>();
        services.AddDocumentManagementApi(
            configuration: configuration,
            builder: builder);
    }

    public static void AddDocumentManagementHostedServices(
        this IServiceCollection services,
        Action<DocumentManagementConfiguration> configure = null)
    {
        DocumentManagementConfiguration configuration = DocumentManagementConfigurationFactory.CreateDocumentManagementConfiguration();
        configure?.Invoke(obj: configuration);
        services.AddDocumentManagementHostedServices(configuration: configuration);
    }

    public static void AddDocumentManagementHostedServices(
        this IServiceCollection services,
        DocumentManagementConfiguration configuration)
    {
        services.RegisterDocumentManagementConfiguration(
            configuration: configuration);
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddEventHandlers();
    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<App>();
        services.AddEventingForType<FileContent>();
        services.AddEventingForType<Package>();
        services.AddEventingForType<PackageItem>();
        services.AddEventingForType<DocumentManagementPackageEvent>();
        services.AddEventingForType<cCoder.Data.Models.DMS.File>();
        services.AddEventingForType<Folder>();
        services.AddEventingForType<cCoder.Data.Models.Security.FolderRole>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<Brokers.Loggings.ILoggingBroker, Brokers.Loggings.LoggingBroker>();
        services.AddTransient<IDmsInstanceFactory, DmsInstanceFactory>();
        services.AddTransient<IDmsFactoryExposure, DmsInstanceFactory>();
        services.AddTransient<IDms, Dms>();
        services.AddTransient<IAuthInfoBroker, AuthInfoBroker>();
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
        services.AddTransient<IHttpContextBroker, HttpContextBroker>();
        services.AddTransient<IMetadataContainerBroker, MetadataContainerBroker>();
        services.AddTransient<IStreamBroker, StreamBroker>();
        services.AddTransient<IDocumentArchiveBroker, DocumentArchiveBroker>();
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<IDocumentManagementAppExposure, DocumentManagementAppExposure>();
        services.AddTransient<IDmsInstanceOperationsExposure, DmsInstanceOperationsExposure>();
        services.AddTransient<IDmsRequestOperationsExposure, DmsRequestOperationsExposure>();
        services.AddTransient<IWebDavOperationsExposure, WebDavOperationsExposure>();
        services.AddTransient<IFileContentOperationsExposure, FileContentOperationsExposure>();
        services.AddTransient<IFileOperationsExposure, FileOperationsExposure>();
        services.AddTransient<IFileMutationOperationsExposure, FileMutationOperationsExposure>();
        services.AddTransient<IFilePathOperationsExposure, FilePathOperationsExposure>();
        services.AddTransient<IFolderPathOperationsExposure, FolderPathOperationsExposure>();
        services.AddTransient<IFolderOperationsExposure, FolderOperationsExposure>();
        services.AddTransient<IFolderMutationOperationsExposure, FolderMutationOperationsExposure>();
        services.AddTransient<IFolderRoleOperationsExposure, FolderRoleOperationsExposure>();
        services.AddTransient<IRoleOperationsExposure, RoleOperationsExposure>();
        services.AddTransient<IDocumentManagementPackageManager, DocumentManagementPackageManager>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
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
        services.AddTransient<ICurrentAppResolverService, CurrentAppResolverService>();
        services.AddTransient<IPackagePayloadJsonService, PackagePayloadJsonService>();
        services.AddTransient<IPackagePayloadShapeService, PackagePayloadShapeService>();
        services.AddTransient<ICurrentAppResolverProcessingService, CurrentAppResolverProcessingService>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<IAppAggregationService, AppAggregationService>();
        services.AddTransient<IDocumentManagementMigrationAggregationService, DocumentManagementMigrationAggregationService>();
        services.AddTransient<IDmsAggregationService, DmsAggregationService>();
        services.AddTransient<IDmsHttpBroker, DmsHttpBroker>();
        services.AddTransient<IDmsHttpService, DmsHttpService>();
        services.AddTransient<IDmsHttpProcessingService, DmsHttpProcessingService>();
        services.AddTransient<IDmsHttpRequestAggregationService, DmsHttpRequestAggregationService>();
        services.AddTransient<IDmsHttpRequestManager, DmsHttpRequestAggregationService>();
        services.AddTransient<IFileContentOrchestrationService, FileContentOrchestrationService>();
        services.AddTransient<IFileContentManager, FileContentOrchestrationService>();
        services.AddTransient<IFileMutationAggregationService, FileMutationAggregationService>();
        services.AddTransient<FileMutationAggregationService>();
        services.AddTransient<IFileManager, FileMutationAggregationService>();
        services.AddTransient<IFolderMutationAggregationService, FolderMutationAggregationService>();
        services.AddTransient<FolderMutationAggregationService>();
        services.AddTransient<IFolderManager, FolderMutationAggregationService>();
        services.AddTransient<IFolderEventManager, FolderEventManager>();
        services.AddTransient<IFolderRoleOrchestrationService, FolderRoleOrchestrationService>();
        services.AddTransient<IFolderRoleManager, FolderRoleOrchestrationService>();
        services.AddTransient<IPackagePayloadMigrationOrchestrationService, PackagePayloadMigrationOrchestrationService>();
        services.AddTransient<IRoleMigrationOrchestrationService, RoleMigrationOrchestrationService>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<IDmsRequestAggregationService, DmsRequestAggregationService>();
        services.AddTransient<IFileContentEventProcessingService, FileContentEventProcessingService>();
        services.AddTransient<IFileContentProcessingService, FileContentProcessingService>();
        services.AddTransient<IFolderRoleEventProcessingService, FolderRoleEventProcessingService>();
        services.AddTransient<IFolderRoleProcessingService, FolderRoleProcessingService>();
        services.AddTransient<IRoleMigrationFilterProcessingService, RoleMigrationFilterProcessingService>();
        services.AddTransient<IRoleMigrationRetrievalProcessingService, RoleMigrationRetrievalProcessingService>();
        services.AddTransient<IWebDavAggregationService, WebDavAggregationService>();
    }
}