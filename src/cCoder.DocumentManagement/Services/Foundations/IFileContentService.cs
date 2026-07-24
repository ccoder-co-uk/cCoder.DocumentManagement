// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;


namespace cCoder.DocumentManagement.Services.Foundations;

public interface IFileContentService
{
    FileContent Get(Guid fileContentId);
    IQueryable<FileContent> GetAll(bool ignoreFilters = false);
    ValueTask DeleteAllForFileAsync(Guid fileId);
    ValueTask DeleteAllForFilesAsync(Guid[] fileIds);
    ValueTask<FileContent> AddFileContentAsync(FileContent newFileContent);
    ValueTask<FileContent> UpdateFileContentAsync(FileContent updatedFileContent);
    ValueTask DeleteAsync(Guid fileContentId);
}