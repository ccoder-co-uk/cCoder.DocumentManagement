// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.DocumentManagement.Brokers.Events;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Aggregations;
using cCoder.DocumentManagement.Services.Coordinations;
using cCoder.DocumentManagement.Services.Orchestrations;
using DataFolder = cCoder.Data.Models.DMS.Folder;
using DataPackageItem = cCoder.Data.Models.Packaging.PackageItem;
using DmsFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal partial class EventHandlerService(IEventHubBroker eventHubBroker) : IEventHandlerService
{
    public void ListenToAllEvents()
=>
        TryCatch(operation: () =>
        {

            ListenToFileSystemEvents();

            ListenToPackageEvents();

        });

    void ListenToFileSystemEvents()
    {
        ListenToAppEvents();
        ListenToFolderEvents();
        ListenToFileEvents();
    }

    void ListenToAppEvents()
    {
        ListenToAppAddEvents();
        ListenToAppUpdateEvents();
        ListenToAppDeleteEvents();
    }

    void ListenToFolderEvents() =>
        ListenToFolderDeleteEvents();

    void ListenToFileEvents() =>
        ListenToFileDeleteEvents();

    void ListenToPackageEvents() =>
        ListenToPackageImportEvents();

    void ListenToAppAddEvents() =>
        eventHubBroker.ListenToEvent<App, IAppAggregationService>(
            eventName: "app_add",
            handler: (service, app) => service.AddAppAsync(newApp: app));

    void ListenToAppUpdateEvents() =>
        eventHubBroker.ListenToEvent<App, IAppAggregationService>(
            eventName: "app_update",
            handler: (service, app) => service.UpdateAppAsync(updatedApp: app));

    void ListenToAppDeleteEvents() =>
        eventHubBroker.ListenToEvent<App, IAppAggregationService>(
            eventName: "app_delete",
            handler: (service, app) => service.DeleteAsync(appId: app.Id));

    void ListenToFolderDeleteEvents() =>
        eventHubBroker.ListenToEvent<DataFolder, IFolderCoordinationService>(
            eventName: "folder_delete",
            handler: (service, folder) => service.DeleteFolderAsync(deletedFolder: folder));

    void ListenToFileDeleteEvents() =>
        eventHubBroker.ListenToEvent<DmsFile, IFileOrchestrationService>(
            eventName: "file_delete",
            handler: (service, file) => service.HandleFileDeleteEventAsync(file: file));

    void ListenToPackageImportEvents() =>
        eventHubBroker.ListenToEvent<(int appId, Package package), IDocumentManagementMigrationAggregationService>(
            eventName: "package_import",
            handler: (service, args) => service.ImportPackageDocumentManagementPackageAsync(appId: args.appId, package: ToLocalPackage(package: args.package)));

    static DocumentManagementPackage ToLocalPackage(Package package) =>
        package == null ? null : new DocumentManagementPackage(name: package.Name)
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Category = package.Category,
            SourceApi = package.SourceApi,
            Items = package.Items?.Select(selector: ToLocalPackageItem)
                                                                       .ToArray(),
        };

    static DocumentManagementPackageItem ToLocalPackageItem(DataPackageItem packageItem) =>
        packageItem == null ? null : new DocumentManagementPackageItem
        {
            Id = packageItem.Id,
            PackageId = packageItem.PackageId,
            Type = packageItem.Type,
            Data = packageItem.Data,
        };

}