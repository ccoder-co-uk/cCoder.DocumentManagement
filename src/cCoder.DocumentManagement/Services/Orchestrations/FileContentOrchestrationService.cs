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
    public FileContent Get(Guid id)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [id]);
            return processingService.Get(id: id);

        });

    public IQueryable<FileContent> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return processingService.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<FileContent> AddFileContentAsync(FileContent entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            FileContent result = await processingService.AddFileContentAsync(entity: entity);

            await eventService.RaiseFileContentAddEventAsync(entity: result);

            return result;

        });

    public ValueTask<FileContent> UpdateFileContentAsync(FileContent entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            FileContent result = await processingService.UpdateFileContentAsync(entity: entity);

            await eventService.RaiseFileContentUpdateEventAsync(entity: result);

            return result;

        });

    public ValueTask DeleteAsync(Guid id)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [id]);
            FileContent entity = processingService.Get(id: id);

            await eventService.RaiseFileContentDeleteEventAsync(entity: entity);

            await processingService.DeleteAsync(id: id);

        });

    public ValueTask<IEnumerable<Result<FileContent>>> AddOrUpdateFileContent(IEnumerable<FileContent> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.AddOrUpdateFileContent(items: items);

        });

    public ValueTask DeleteAllFileContentAsync(IEnumerable<FileContent> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.DeleteAllFileContentAsync(items: items);

        });
}