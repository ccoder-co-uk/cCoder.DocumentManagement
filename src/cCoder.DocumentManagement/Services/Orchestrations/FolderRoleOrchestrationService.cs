using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Processings;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal class FolderRoleOrchestrationService(IFolderRoleProcessingService processingService, IFolderRoleEventProcessingService eventService) : IFolderRoleOrchestrationService
{
    public IQueryable<FolderRole> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<FolderRole> AddAsync(FolderRole entity)
    {
        FolderRole result = await processingService.AddAsync(entity);
        await eventService.RaiseFolderRoleAddEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(FolderRole entity)
    {
        await eventService.RaiseFolderRoleDeleteEventAsync(entity);
        await processingService.DeleteAsync(entity);
    }

    public ValueTask<IEnumerable<Result<FolderRole>>> AddOrUpdate(IEnumerable<FolderRole> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<FolderRole> items)
    {
        return processingService.DeleteAllAsync(items);
    }
}
