// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Processings;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal class FolderOrchestrationService(IFolderProcessingService processingService, IFolderEventProcessingService eventService) : IFolderOrchestrationService
{
    public Folder Get(Guid id)
    {
        return processingService.Get(id: id);
    }

    public IQueryable<Folder> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public async ValueTask<Folder> AddAsync(Folder entity)
    {
        Folder result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseFolderAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<Folder> UpdateAsync(Folder entity)
    {
        Folder result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseFolderUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        Folder entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: folder => folder.Id == id);

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseFolderDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public ValueTask<IEnumerable<Result<Folder>>> AddOrUpdate(IEnumerable<Folder> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateForAppAsync(IEnumerable<Folder> items)
    {
        return processingService.AddOrUpdateForAppAsync(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<Folder> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }

    public ValueTask DeleteAllByAppIdAsync(int appId)
    {
        return processingService.DeleteByAppIdAsync(appId: appId);
    }

    public ValueTask<List<Result<Guid?>>> CopyAsync(string source, string destination, int sourceAppId, int destAppId)
    {
        return processingService.CopyAsync(source: source, destination: destination, sourceAppId: sourceAppId, destAppId: destAppId);
    }

    public ValueTask HandleFolderDeleteEventAsync(Folder folder)
    {
        return processingService.HandleFolderDeleteEventAsync(folder: folder);
    }

    public DMSResult GetFilesZipped(App app, IEnumerable<cCoder.DocumentManagement.Models.Path> paths)
    {
        return processingService.GetFilesZipped(app: app, paths: paths);
    }

    public DMSResult Get(App app, cCoder.DocumentManagement.Models.Path path, string search = "")
    {
        return processingService.Get(app: app, path: path, search: search);
    }

    public ValueTask UnpackAsync(App app, cCoder.DocumentManagement.Models.Path path, Stream content, bool ignoreArchiveRoot = false)
    {
        return processingService.UnpackAsync(app: app, path: path, content: content, ignoreArchiveRoot: ignoreArchiveRoot);
    }

    public ValueTask SaveAsync(App app, cCoder.DocumentManagement.Models.Path path)
    {
        return processingService.SaveAsync(app: app, path: path);
    }

    public ValueTask DropAsync(App app, cCoder.DocumentManagement.Models.Path path)
    {
        return processingService.DropAsync(app: app, path: path);
    }

    public ValueTask CopyAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        return processingService.CopyAsync(app: app, oldPath: oldPath, newPath: newPath);
    }

    public ValueTask MoveAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        return processingService.MoveAsync(app: app, oldPath: oldPath, newPath: newPath);
    }
}