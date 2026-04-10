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

internal class EventHandlerService(IEventHubBroker eventHubBroker) : IEventHandlerService
{
    public void ListenToAllEvents()
    {
        ListenToFileSystemEvents();
        ListenToPackageEvents();
    }

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

    void ListenToFolderEvents() => ListenToFolderDeleteEvents();

    void ListenToFileEvents() => ListenToFileDeleteEvents();

    void ListenToPackageEvents() => ListenToPackageImportEvents();

    void ListenToAppAddEvents() =>
        eventHubBroker.ListenToEvent<App, IAppOrchestrationService>(
            "app_add",
            (service, app) => service.AddAsync(app));

    void ListenToAppUpdateEvents() =>
        eventHubBroker.ListenToEvent<App, IAppOrchestrationService>(
            "app_update",
            (service, app) => service.UpdateAsync(app));

    void ListenToAppDeleteEvents() =>
        eventHubBroker.ListenToEvent<App, IAppOrchestrationService>(
            "app_delete",
            (service, app) => service.DeleteAsync(app.Id));

    void ListenToFolderDeleteEvents() =>
        eventHubBroker.ListenToEvent<DataFolder, IFolderCoordinationService>(
            "folder_delete",
            (service, folder) => service.DeleteFolderAsync(folder));

    void ListenToFileDeleteEvents() =>
        eventHubBroker.ListenToEvent<DmsFile, IFileOrchestrationService>(
            "file_delete",
            (service, file) => service.HandleFileDeleteEventAsync(file));

    void ListenToPackageImportEvents() =>
        eventHubBroker.ListenToEvent<(int appId, Package package), IDocumentManagementMigrationAggregationService>(
            "package_import",
            (service, args) => service.ImportPackageAsync(args.appId, ToLocalPackage(args.package)));

    static DocumentManagementPackage ToLocalPackage(Package package) =>
        package == null ? null : new DocumentManagementPackage(package.Name)
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Category = package.Category,
            SourceApi = package.SourceApi,
            Items = package.Items?.Select(ToLocalPackageItem).ToArray(),
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



