// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Processings;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal partial class FileContentOrchestrationService(IFileContentProcessingService processingService, IFileContentEventProcessingService eventService) : IFileContentOrchestrationService
{
    public FileContent Get(Guid fileContentId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileContentId]);
            return processingService.Get(fileContentId: fileContentId);

        });

    public IQueryable<FileContent> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return processingService.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<FileContent> AddFileContentAsync(FileContent newFileContent)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newFileContent]);
            FileContent result = await processingService.AddFileContentAsync(newFileContent: newFileContent);

            await eventService.RaiseFileContentAddEventAsync(entity: result);

            return result;

        });

    public ValueTask<FileContent> UpdateFileContentAsync(FileContent updatedFileContent)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedFileContent]);
            FileContent result = await processingService.UpdateFileContentAsync(updatedFileContent: updatedFileContent);

            await eventService.RaiseFileContentUpdateEventAsync(entity: result);

            return result;

        });

    public ValueTask DeleteAsync(Guid fileContentId)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [fileContentId]);
            FileContent entity = processingService.Get(fileContentId: fileContentId);

            await eventService.RaiseFileContentDeleteEventAsync(entity: entity);

            await processingService.DeleteAsync(fileContentId: fileContentId);

        });

    public ValueTask<IEnumerable<Result<FileContent>>> AddOrUpdateFileContent(IEnumerable<FileContent> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.AddOrUpdateFileContent(items: items);

        });

    public ValueTask DeleteAllFileContentAsync(IEnumerable<FileContent> deletedFileContent)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [deletedFileContent]);
            return processingService.DeleteAllFileContentAsync(deletedFileContent: deletedFileContent);

        });
}