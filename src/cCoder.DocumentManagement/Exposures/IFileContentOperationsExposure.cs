// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Exposures;

public interface IFileContentOperationsExposure
{
    IQueryable<FileContent> GetAll(bool ignoreFilters = false);

    ValueTask<FileContent> AddFileContentAsync(
        FileContent newFileContent);

    ValueTask DeleteFileContentAsync(Guid fileContentId);

    ValueTask DeleteAllForFileAsync(Guid fileId);

    ValueTask DeleteAllForFilesAsync(IEnumerable<Guid> fileIds);

    ValueTask<IEnumerable<Result<FileContent>>> AddOrUpdateFileContent(
        IEnumerable<FileContent> items);
}