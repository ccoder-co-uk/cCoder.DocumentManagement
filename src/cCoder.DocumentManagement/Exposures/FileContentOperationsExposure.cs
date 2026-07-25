// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Services.Processings;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class FileContentOperationsExposure(
    IFileContentProcessingService fileContentProcessingService)
    : IFileContentOperationsExposure
{
    public IQueryable<FileContent> GetAll(bool ignoreFilters = false) =>
        fileContentProcessingService.GetAll(
            ignoreFilters: ignoreFilters);

    public ValueTask<FileContent> AddFileContentAsync(
        FileContent newFileContent) =>
        fileContentProcessingService.AddFileContentAsync(
            newFileContent: newFileContent);

    public ValueTask DeleteFileContentAsync(Guid fileContentId) =>
        fileContentProcessingService.DeleteAsync(
            fileContentId: fileContentId);

    public ValueTask DeleteAllForFileAsync(Guid fileId) =>
        fileContentProcessingService.DeleteAllForFileAsync(
            fileId: fileId);

    public ValueTask DeleteAllForFilesAsync(IEnumerable<Guid> fileIds) =>
        fileContentProcessingService.DeleteAllForFilesAsync(
            fileIds: fileIds);

    public ValueTask<IEnumerable<Result<FileContent>>> AddOrUpdateFileContent(
        IEnumerable<FileContent> items) =>
        fileContentProcessingService.AddOrUpdateFileContent(
            items: items);
}