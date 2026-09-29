// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Services.Processings;

internal interface IFileContentProcessingService
{
    FileContent Get(Guid fileContentId);

    IQueryable<FileContent> GetAll(bool ignoreFilters = false);

    ValueTask<FileContent> AddFileContentAsync(FileContent newFileContent);

    ValueTask<FileContent> UpdateFileContentAsync(FileContent updatedFileContent);

    ValueTask DeleteAsync(Guid fileContentId);

    ValueTask DeleteAllForFileAsync(Guid fileId);

    ValueTask DeleteAllForFilesAsync(IEnumerable<Guid> fileIds);

    ValueTask<IEnumerable<Result<FileContent>>> AddOrUpdateFileContent(IEnumerable<FileContent> items);

    ValueTask DeleteAllFileContentAsync(IEnumerable<FileContent> deletedFileContent);
}