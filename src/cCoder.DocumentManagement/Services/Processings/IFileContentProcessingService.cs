using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Processings;

public interface IFileContentProcessingService
{
    FileContent Get(Guid id);

    IQueryable<FileContent> GetAll(bool ignoreFilters = false);

    ValueTask<FileContent> AddAsync(FileContent entity);

    ValueTask<FileContent> UpdateAsync(FileContent entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<Result<FileContent>>> AddOrUpdate(IEnumerable<FileContent> items);

    ValueTask DeleteAllAsync(IEnumerable<FileContent> items);
}
