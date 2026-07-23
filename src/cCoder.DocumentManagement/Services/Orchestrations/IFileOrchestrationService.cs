// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Orchestrations;

public interface IFileOrchestrationService
{
    cCoder.Data.Models.DMS.File Get(Guid id);

    IQueryable<cCoder.Data.Models.DMS.File> GetAll(bool ignoreFilters = false);

    ValueTask<cCoder.Data.Models.DMS.File> AddAsync(cCoder.Data.Models.DMS.File entity);

    ValueTask<cCoder.Data.Models.DMS.File> UpdateAsync(cCoder.Data.Models.DMS.File entity);

    ValueTask DeleteAsync(Guid id);
    ValueTask<IEnumerable<Result<cCoder.Data.Models.DMS.File>>> AddOrUpdate(IEnumerable<cCoder.Data.Models.DMS.File> items);

    ValueTask DeleteAllAsync(IEnumerable<cCoder.Data.Models.DMS.File> items);

    cCoder.Data.Models.DMS.File GetByPath(int appId, string path);

    ValueTask HandleFileDeleteEventAsync(cCoder.Data.Models.DMS.File file);

    DMSResult Get(App app, cCoder.DocumentManagement.Models.Path path, int version = 0);

    IEnumerable<cCoder.Data.Models.DMS.File> Search(App app, string needle);

    ValueTask SaveAsync(App app, cCoder.DocumentManagement.Models.Path path, Stream content = null);

    ValueTask DropAsync(App app, cCoder.DocumentManagement.Models.Path path, int version = 0);

    ValueTask CopyAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath);

    ValueTask MoveAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath);
}