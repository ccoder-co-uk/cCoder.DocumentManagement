// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Exposures;

public interface IFileManager
{
    cCoder.Data.Models.DMS.File Get(Guid fileId);

    IQueryable<cCoder.Data.Models.DMS.File> GetAll(bool ignoreFilters = false);

    ValueTask<cCoder.Data.Models.DMS.File> AddFileAsync(cCoder.Data.Models.DMS.File newFile);

    ValueTask<cCoder.Data.Models.DMS.File> UpdateFileAsync(cCoder.Data.Models.DMS.File updatedFile);

    ValueTask DeleteAsync(Guid fileId);
    ValueTask<IEnumerable<Result<cCoder.Data.Models.DMS.File>>> AddOrUpdateFile(IEnumerable<cCoder.Data.Models.DMS.File> items);

    ValueTask DeleteAllFileAsync(IEnumerable<cCoder.Data.Models.DMS.File> deletedFile);

    cCoder.Data.Models.DMS.File GetByPath(int appId, string path);

    ValueTask HandleFileDeleteEventAsync(cCoder.Data.Models.DMS.File file);

}