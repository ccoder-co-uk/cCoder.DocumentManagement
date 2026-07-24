// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Processings;

public interface IFileContentProcessingService
{
    FileContent Get(Guid fileContentId);

    IQueryable<FileContent> GetAll(bool ignoreFilters = false);

    ValueTask<FileContent> AddFileContentAsync(FileContent newFileContent);

    ValueTask<FileContent> UpdateFileContentAsync(FileContent updatedFileContent);

    ValueTask DeleteAsync(Guid fileContentId);

    ValueTask<IEnumerable<Result<FileContent>>> AddOrUpdateFileContent(IEnumerable<FileContent> items);

    ValueTask DeleteAllFileContentAsync(IEnumerable<FileContent> deletedFileContent);
}