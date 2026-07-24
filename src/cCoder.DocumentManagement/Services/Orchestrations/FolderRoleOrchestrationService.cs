// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Processings;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal partial class FolderRoleOrchestrationService(IFolderRoleProcessingService processingService, IFolderRoleEventProcessingService eventService) : IFolderRoleOrchestrationService
{
    public IQueryable<FolderRole> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return processingService.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<FolderRole> AddAsync(FolderRole entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            FolderRole result = await processingService.AddAsync(entity: entity);

            await eventService.RaiseFolderRoleAddEventAsync(entity: result);

            return result;

        });

    public ValueTask DeleteAsync(FolderRole entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            await eventService.RaiseFolderRoleDeleteEventAsync(entity: entity);

            await processingService.DeleteAsync(entity: entity);

        });

    public ValueTask<IEnumerable<Result<FolderRole>>> AddOrUpdate(IEnumerable<FolderRole> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.AddOrUpdate(items: items);

        });

    public ValueTask DeleteAllAsync(IEnumerable<FolderRole> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.DeleteAllAsync(items: items);

        });
}