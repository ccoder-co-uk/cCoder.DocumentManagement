// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Processings;

public interface IFileProcessingService
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

    DMSResult GetAppPath(App app, cCoder.DocumentManagement.Dependencies.Path path, int version = 0);

    IEnumerable<cCoder.Data.Models.DMS.File> SearchApp(App app, string needle);

    ValueTask SaveAppPathAsync(App app, cCoder.DocumentManagement.Dependencies.Path path, Stream content = null);

    ValueTask DropAppPathAsync(App app, cCoder.DocumentManagement.Dependencies.Path path, int version = 0);

    ValueTask CopyAppPathAsync(App app, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath);

    ValueTask MoveAppPathAsync(App app, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath);
}