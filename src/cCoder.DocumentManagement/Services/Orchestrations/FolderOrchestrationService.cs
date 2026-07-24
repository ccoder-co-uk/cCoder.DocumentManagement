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

    public ValueTask<Folder> AddAsync(Folder entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            Folder result = await processingService.AddAsync(entity: entity);

            await eventService.RaiseFolderAddEventAsync(entity: result);

            return result;

        });

    public ValueTask<Folder> UpdateAsync(Folder entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            Folder result = await processingService.UpdateAsync(entity: entity);

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

    public ValueTask<IEnumerable<Result<Folder>>> AddOrUpdate(IEnumerable<Folder> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.AddOrUpdate(items: items);

        });

    public ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateForAppAsync(IEnumerable<Folder> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.AddOrUpdateForAppAsync(items: items);

        });

    public ValueTask DeleteAllAsync(IEnumerable<Folder> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.DeleteAllAsync(items: items);

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

    public DMSResult GetFilesZipped(App app, IEnumerable<cCoder.DocumentManagement.Models.Path> paths)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, paths]);
            return processingService.GetFilesZipped(app: app, paths: paths);

        });

    public DMSResult Get(App app, cCoder.DocumentManagement.Models.Path path, string search = "")
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, path, search]);
            return processingService.Get(app: app, path: path, search: search);

        });

    public ValueTask UnpackAsync(App app, cCoder.DocumentManagement.Models.Path path, Stream content, bool ignoreArchiveRoot = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, path, content, ignoreArchiveRoot]);
            return processingService.UnpackAsync(app: app, path: path, content: content, ignoreArchiveRoot: ignoreArchiveRoot);

        });

    public ValueTask SaveAsync(App app, cCoder.DocumentManagement.Models.Path path)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, path]);
            return processingService.SaveAsync(app: app, path: path);

        });

    public ValueTask DropAsync(App app, cCoder.DocumentManagement.Models.Path path)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, path]);
            return processingService.DropAsync(app: app, path: path);

        });

    public ValueTask CopyAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, oldPath, newPath]);
            return processingService.CopyAsync(app: app, oldPath: oldPath, newPath: newPath);

        });

    public ValueTask MoveAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, oldPath, newPath]);
            return processingService.MoveAsync(app: app, oldPath: oldPath, newPath: newPath);

        });
}