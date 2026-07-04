using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Processings;

public interface IFolderProcessingService
{
    Folder Get(Guid id);

    IQueryable<Folder> GetAll(bool ignoreFilters = false);

    ValueTask<Folder> AddAsync(Folder entity);

    ValueTask<Folder> UpdateAsync(Folder entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<Result<Folder>>> AddOrUpdate(IEnumerable<Folder> items);

    ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateForAppAsync(IEnumerable<Folder> items);

    ValueTask DeleteAllAsync(IEnumerable<Folder> items);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<List<Result<Guid?>>> CopyAsync(string source, string destination, int sourceAppId, int destAppId);

    ValueTask HandleFolderDeleteEventAsync(Folder folder);

    DMSResult GetFilesZipped(App app, IEnumerable<cCoder.DocumentManagement.Models.Path> paths);

    DMSResult Get(App app, cCoder.DocumentManagement.Models.Path path, string search = "");

    ValueTask UnpackAsync(App app, cCoder.DocumentManagement.Models.Path path, Stream content, bool ignoreArchiveRoot = false);

    ValueTask SaveAsync(App app, cCoder.DocumentManagement.Models.Path path);

    ValueTask DropAsync(App app, cCoder.DocumentManagement.Models.Path path);

    ValueTask CopyAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath);

    ValueTask MoveAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath);
}
