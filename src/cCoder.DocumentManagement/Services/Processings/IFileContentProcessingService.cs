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
    FileContent Get(Guid id);

    IQueryable<FileContent> GetAll(bool ignoreFilters = false);

    ValueTask<FileContent> AddFileContentAsync(FileContent entity);

    ValueTask<FileContent> UpdateFileContentAsync(FileContent entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<Result<FileContent>>> AddOrUpdateFileContent(IEnumerable<FileContent> items);

    ValueTask DeleteAllFileContentAsync(IEnumerable<FileContent> items);
}