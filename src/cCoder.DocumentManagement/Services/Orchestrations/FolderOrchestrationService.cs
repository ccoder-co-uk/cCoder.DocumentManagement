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
    public Folder Get(Guid id)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [id]);
            return processingService.Get(id: id);

        });

    public IQueryable<Folder> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return processingService.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<Folder> AddFolderAsync(Folder entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            Folder result = await processingService.AddFolderAsync(entity: entity);

            await eventService.RaiseFolderAddEventAsync(entity: result);

            return result;

        });

    public ValueTask<Folder> UpdateFolderAsync(Folder entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            Folder result = await processingService.UpdateFolderAsync(entity: entity);

            await eventService.RaiseFolderUpdateEventAsync(entity: result);

            return result;

        });

    public ValueTask DeleteAsync(Guid id)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [id]);
            Folder entity = processingService.GetAll(ignoreFilters: true)
    .FirstOrDefault(predicate: folder => folder.Id == id);


            if (entity == null)
            {
                return;
            }


            await eventService.RaiseFolderDeleteEventAsync(entity: entity);

            await processingService.DeleteAsync(id: id);

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

    public ValueTask DeleteAllFolderAsync(IEnumerable<Folder> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.DeleteAllFolderAsync(items: items);

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

    public DMSResult GetFilesZippedAppPath(App app, IEnumerable<cCoder.DocumentManagement.Models.Path> paths)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, paths]);
            return processingService.GetFilesZippedAppPath(app: app, paths: paths);

        });

    public DMSResult GetAppPath(App app, cCoder.DocumentManagement.Models.Path path, string search = "")
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, path, search]);
            return processingService.GetAppPath(app: app, path: path, search: search);

        });

    public ValueTask UnpackAppPathAsync(App app, cCoder.DocumentManagement.Models.Path path, Stream content, bool ignoreArchiveRoot = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, path, content, ignoreArchiveRoot]);
            return processingService.UnpackAppPathAsync(app: app, path: path, content: content, ignoreArchiveRoot: ignoreArchiveRoot);

        });

    public ValueTask SaveAppPathAsync(App app, cCoder.DocumentManagement.Models.Path path)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, path]);
            return processingService.SaveAppPathAsync(app: app, path: path);

        });

    public ValueTask DropAppPathAsync(App app, cCoder.DocumentManagement.Models.Path path)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, path]);
            return processingService.DropAppPathAsync(app: app, path: path);

        });

    public ValueTask CopyAppPathAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, oldPath, newPath]);
            return processingService.CopyAppPathAsync(app: app, oldPath: oldPath, newPath: newPath);

        });

    public ValueTask MoveAppPathAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, oldPath, newPath]);
            return processingService.MoveAppPathAsync(app: app, oldPath: oldPath, newPath: newPath);

        });
}