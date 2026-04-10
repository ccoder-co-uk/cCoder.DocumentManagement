using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;

namespace cCoder.DocumentManagement.Services.Processings;

internal class FileContentProcessingService(IFileContentService service) : IFileContentProcessingService
{
    public FileContent Get(Guid id)
    {
        return service.Get(id);
    }

    public IQueryable<FileContent> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<FileContent> AddAsync(FileContent entity)
    {
        return service.AddAsync(entity);
    }

    public ValueTask<FileContent> UpdateAsync(FileContent entity)
    {
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(Guid id)
    {
        return service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<Result<FileContent>>> AddOrUpdate(IEnumerable<FileContent> items)
    {
        List<Result<FileContent>> results = new List<Result<FileContent>>();

        foreach (FileContent item in items)
        {
            try
            {
                FileContent savedItem = item.Id == Guid.Empty ? await AddAsync(item) : await UpdateAsync(item);

                results.Add(new Result<FileContent>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<FileContent>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<FileContent> items)
    {
        foreach (FileContent item in items)
        {
            await DeleteAsync(item.Id);
        }
    }
}
