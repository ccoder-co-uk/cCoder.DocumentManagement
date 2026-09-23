// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Aggregations;
using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.Eventing;
using DataFolder = cCoder.Data.Models.DMS.Folder;
using DataPackageItem = cCoder.Data.Models.Packaging.PackageItem;
using DmsFile = cCoder.Data.Models.DMS.File;

namespace cCoder.DocumentManagement;

public static class IEventHubExtensions
{
    public static IEventHub ListenToDocumentManagementEvents(this IEventHub eventHub)
    {
        ListenToAppEvents(eventHub: eventHub);
        ListenToFolderEvents(eventHub: eventHub);
        ListenToFileEvents(eventHub: eventHub);
        ListenToPackageEvents(eventHub: eventHub);

        return eventHub;
    }

    private static void ListenToAppEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent<App, IAppAggregationService>(
            name: "app_add",
            handler: (service, app) => service.AddAppAsync(newApp: app));

        eventHub.ListenToEvent<App, IAppAggregationService>(
            name: "app_update",
            handler: (service, app) => service.UpdateAppAsync(updatedApp: app));

        eventHub.ListenToEvent<App, IAppAggregationService>(
            name: "app_delete",
            handler: (service, app) => service.DeleteAsync(appId: app.Id));
    }

    private static void ListenToFolderEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent<DataFolder, IFolderMutationAggregationService>(
            name: "folder_delete",
            handler: (service, folder) => service.HandleFolderDeleteEventAsync(folder: folder));

    private static void ListenToFileEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent<DmsFile, IFileMutationAggregationService>(
            name: "file_delete",
            handler: (service, file) => service.HandleFileDeleteEventAsync(file: file));

    private static void ListenToPackageEvents(IEventHub eventHub) =>
        eventHub.ListenToEvent<DocumentManagementPackageEvent, IDocumentManagementMigrationAggregationService>(
            name: "package_import",
            handler: (service, packageEvent) => service.ImportPackageDocumentManagementPackageAsync(
                appId: packageEvent.AppId,
                documentManagementPackage: ToLocalPackage(package: packageEvent.Package)));

    private static DocumentManagementPackage ToLocalPackage(Package package) =>
        package == null ? null : new DocumentManagementPackage
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Category = package.Category,
            SourceApi = package.SourceApi,
            Items = package.Items?
                .Select(selector: ToLocalPackageItem)
                .ToArray(),
        };

    private static DocumentManagementPackageItem ToLocalPackageItem(DataPackageItem packageItem) =>
        packageItem == null ? null : new DocumentManagementPackageItem
        {
            Id = packageItem.Id,
            PackageId = packageItem.PackageId,
            Type = packageItem.Type,
            Data = packageItem.Data,
        };
}