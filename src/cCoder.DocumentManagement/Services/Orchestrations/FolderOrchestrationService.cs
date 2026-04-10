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
        return processingService.Get(id);
    }

    public IQueryable<Folder> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<Folder> AddAsync(Folder entity)
    {
        Folder result = await processingService.AddAsync(entity);
        await eventService.RaiseFolderAddEventAsync(result);
        return result;
    }

    public async ValueTask<Folder> UpdateAsync(Folder entity)
    {
        Folder result = await processingService.UpdateAsync(entity);
        await eventService.RaiseFolderUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        Folder entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(folder => folder.Id == id);

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseFolderDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result<Folder>>> AddOrUpdate(IEnumerable<Folder> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<Folder> items)
    {
        return processingService.DeleteAllAsync(items);
    }

    public ValueTask<List<Result<Guid?>>> CopyAsync(string source, string destination, int sourceAppId, int destAppId)
    {
        return processingService.CopyAsync(source, destination, sourceAppId, destAppId);
    }

    public ValueTask HandleFolderDeleteEventAsync(Folder folder)
    {
        return processingService.HandleFolderDeleteEventAsync(folder);
    }

    public DMSResult GetFilesZipped(App app, IEnumerable<cCoder.DocumentManagement.Models.Path> paths)
    {
        return processingService.GetFilesZipped(app, paths);
    }

    public DMSResult Get(App app, cCoder.DocumentManagement.Models.Path path, string search = "")
    {
        return processingService.Get(app, path, search);
    }

    public ValueTask UnpackAsync(App app, cCoder.DocumentManagement.Models.Path path, Stream content, bool ignoreArchiveRoot = false)
    {
        return processingService.UnpackAsync(app, path, content, ignoreArchiveRoot);
    }

    public ValueTask SaveAsync(App app, cCoder.DocumentManagement.Models.Path path)
    {
        return processingService.SaveAsync(app, path);
    }

    public ValueTask DropAsync(App app, cCoder.DocumentManagement.Models.Path path)
    {
        return processingService.DropAsync(app, path);
    }

    public ValueTask CopyAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        return processingService.CopyAsync(app, oldPath, newPath);
    }

    public ValueTask MoveAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        return processingService.MoveAsync(app, oldPath, newPath);
    }
}
