using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Processings;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal class FileContentOrchestrationService(IFileContentProcessingService processingService, IFileContentEventProcessingService eventService) : IFileContentOrchestrationService
{
    public FileContent Get(Guid id)
    {
        return processingService.Get(id);
    }

    public IQueryable<FileContent> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<FileContent> AddAsync(FileContent entity)
    {
        FileContent result = await processingService.AddAsync(entity);
        await eventService.RaiseFileContentAddEventAsync(result);
        return result;
    }

    public async ValueTask<FileContent> UpdateAsync(FileContent entity)
    {
        FileContent result = await processingService.UpdateAsync(entity);
        await eventService.RaiseFileContentUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        FileContent entity = processingService.Get(id);
        await eventService.RaiseFileContentDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask<IEnumerable<Result<FileContent>>> AddOrUpdate(IEnumerable<FileContent> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<FileContent> items)
    {
        return processingService.DeleteAllAsync(items);
    }
}
