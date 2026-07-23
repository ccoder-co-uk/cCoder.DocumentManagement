// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Processings;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal class FileOrchestrationService(IFileProcessingService processingService, IFileEventProcessingService eventService) : IFileOrchestrationService
{
    public cCoder.Data.Models.DMS.File Get(Guid id)
    {
        return processingService.Get(id: id);
    }

    public IQueryable<cCoder.Data.Models.DMS.File> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public async ValueTask<cCoder.Data.Models.DMS.File> AddAsync(cCoder.Data.Models.DMS.File entity)
    {
        cCoder.Data.Models.DMS.File result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseFileAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<cCoder.Data.Models.DMS.File> UpdateAsync(cCoder.Data.Models.DMS.File entity)
    {
        cCoder.Data.Models.DMS.File result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseFileUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        cCoder.Data.Models.DMS.File entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: file => file.Id == id);

        if (entity == null)
        {
            return;
        }

        await eventService.RaiseFileDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public ValueTask<IEnumerable<Result<cCoder.Data.Models.DMS.File>>> AddOrUpdate(IEnumerable<cCoder.Data.Models.DMS.File> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<cCoder.Data.Models.DMS.File> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }

    public cCoder.Data.Models.DMS.File GetByPath(int appId, string path)
    {
        return processingService.GetByPath(appId: appId, path: path);
    }

    public ValueTask HandleFileDeleteEventAsync(cCoder.Data.Models.DMS.File file)
    {
        return processingService.HandleFileDeleteEventAsync(file: file);
    }

    public DMSResult Get(App app, cCoder.DocumentManagement.Models.Path path, int version = 0)
    {
        return processingService.Get(app: app, path: path, version: version);
    }

    public IEnumerable<cCoder.Data.Models.DMS.File> Search(App app, string needle)
    {
        return processingService.Search(app: app, needle: needle);
    }

    public ValueTask SaveAsync(App app, cCoder.DocumentManagement.Models.Path path, Stream content = null)
    {
        return processingService.SaveAsync(app: app, path: path, content: content);
    }

    public ValueTask DropAsync(App app, cCoder.DocumentManagement.Models.Path path, int version = 0)
    {
        return processingService.DropAsync(app: app, path: path, version: version);
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