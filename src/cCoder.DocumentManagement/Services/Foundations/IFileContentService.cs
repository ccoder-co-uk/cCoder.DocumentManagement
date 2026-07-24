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
    FileContent Get(Guid id);
    IQueryable<FileContent> GetAll(bool ignoreFilters = false);
    ValueTask DeleteAllForFileAsync(Guid fileId);
    ValueTask DeleteAllForFilesAsync(Guid[] fileIds);
    ValueTask<FileContent> AddFileContentAsync(FileContent fileContent);
    ValueTask<FileContent> UpdateFileContentAsync(FileContent fileContent);
    ValueTask DeleteAsync(Guid id);
}