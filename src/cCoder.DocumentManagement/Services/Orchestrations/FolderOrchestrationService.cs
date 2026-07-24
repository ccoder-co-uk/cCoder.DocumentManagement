// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Processings;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal partial class FolderOrchestrationService(IFolderProcessingService processingService, IFolderEventProcessingService eventService) : IFolderOrchestrationService
{
    public Folder Get(Guid folderId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folderId]);
            return processingService.Get(folderId: folderId);

        });

    public IQueryable<Folder> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return processingService.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<Folder> AddFolderAsync(Folder newFolder)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newFolder]);
            Folder result = await processingService.AddFolderAsync(newFolder: newFolder);

            await eventService.RaiseFolderAddEventAsync(entity: result);

            return result;

        });

    public ValueTask<Folder> UpdateFolderAsync(Folder updatedFolder)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedFolder]);
            Folder result = await processingService.UpdateFolderAsync(updatedFolder: updatedFolder);

            await eventService.RaiseFolderUpdateEventAsync(entity: result);

            return result;

        });

    public ValueTask DeleteAsync(Guid folderId)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [folderId]);

            Folder entity = processingService.GetAll(ignoreFilters: true)
    .FirstOrDefault(predicate: folder => folder.Id == folderId);


            if (entity == null)
            {
                return;
            }


            await eventService.RaiseFolderDeleteEventAsync(entity: entity);

            await processingService.DeleteAsync(folderId: folderId);

        });

    public ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateFolder(IEnumerable<Folder> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.AddOrUpdateFolder(items: items);

        });

    public ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateForAppFolderAsync(IEnumerable<Folder> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.AddOrUpdateForAppFolderAsync(items: items);

        });

    public ValueTask DeleteAllFolderAsync(IEnumerable<Folder> deletedFolder)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [deletedFolder]);
            return processingService.DeleteAllFolderAsync(deletedFolder: deletedFolder);

        });

    public ValueTask DeleteAllByAppIdAsync(int appId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId]);
            return processingService.DeleteByAppIdAsync(appId: appId);

        });

    public ValueTask<List<Result<Guid?>>> CopyAsync(string source, string destination, int sourceAppId, int destAppId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [source, destination, sourceAppId, destAppId]);
            return processingService.CopyAsync(source: source, destination: destination, sourceAppId: sourceAppId, destAppId: destAppId);

        });

    public ValueTask HandleFolderDeleteEventAsync(Folder folder)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folder]);
            return processingService.HandleFolderDeleteEventAsync(folder: folder);

        });

    public DMSResult GetFilesZippedAppPath(int appId, IEnumerable<cCoder.DocumentManagement.Dependencies.Path> paths)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, paths]);
            return processingService.GetFilesZippedAppPath(appId: appId, paths: paths);

        });

    public DMSResult GetAppPath(int appId, cCoder.DocumentManagement.Dependencies.Path path, string search = "")
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, search]);
            return processingService.GetAppPath(appId: appId, path: path, search: search);

        });

    public ValueTask UnpackAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path, Stream content, bool ignoreArchiveRoot = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, content, ignoreArchiveRoot]);
            return processingService.UnpackAppPathAsync(appId: appId, path: path, content: content, ignoreArchiveRoot: ignoreArchiveRoot);

        });

    public ValueTask SaveAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path]);
            return processingService.SaveAppPathAsync(appId: appId, path: path);

        });

    public ValueTask DropAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path]);
            return processingService.DropAppPathAsync(appId: appId, path: path);

        });

    public ValueTask CopyAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, oldPath, newPath]);
            return processingService.CopyAppPathAsync(appId: appId, oldPath: oldPath, newPath: newPath);

        });

    public ValueTask MoveAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, oldPath, newPath]);
            return processingService.MoveAppPathAsync(appId: appId, oldPath: oldPath, newPath: newPath);

        });
}