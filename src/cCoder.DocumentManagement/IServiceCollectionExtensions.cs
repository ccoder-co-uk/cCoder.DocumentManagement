// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Packaging;
using cCoder.DocumentManagement.Api.OData;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Brokers.Events;
using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Exposures.EventHandlers;
using cCoder.DocumentManagement.Models;
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

public static partial class IServiceCollectionExtensions
{
    public static void AddDocumentManagementWeb(
        this IServiceCollection services,
        Action<DocumentManagementConfiguration> configure = null,
        ODataConventionModelBuilder builder = null) =>
        services.AddConfiguredDocumentManagementWeb(configure: (_, configuration) => configure?.Invoke(obj: configuration), builder: builder);

    public static void AddDocumentManagementHostedServices(
        this IServiceCollection services,
        Action<DocumentManagementConfiguration> configure = null) =>
        services.AddConfiguredDocumentManagement(configure: (_, configuration) => configure?.Invoke(obj: configuration));

    private static void AddDocumentManagement(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddEventHandlers();
    }

    private static void AddDocumentManagementWeb(this IServiceCollection services, ODataConventionModelBuilder builder = null)
    {
        services.AddDocumentManagement();

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
}