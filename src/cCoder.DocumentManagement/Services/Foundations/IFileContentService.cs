// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.DMS;


namespace cCoder.DocumentManagement.Services.Foundations;

internal interface IFileContentService
{
    FileContent Get(Guid fileContentId);
    IQueryable<FileContent> GetAll(bool ignoreFilters = false);
    ValueTask DeleteAllForFileAsync(Guid fileId);
    ValueTask DeleteAllForFilesAsync(Guid[] fileIds);
    ValueTask<FileContent> AddFileContentAsync(FileContent newFileContent);
    ValueTask<FileContent> UpdateFileContentAsync(FileContent updatedFileContent);
    ValueTask DeleteAsync(Guid fileContentId);
}