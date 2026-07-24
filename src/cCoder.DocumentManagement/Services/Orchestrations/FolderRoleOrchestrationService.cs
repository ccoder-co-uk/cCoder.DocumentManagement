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

    public ValueTask<FolderRole> AddFolderRoleAsync(FolderRole entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            FolderRole result = await processingService.AddFolderRoleAsync(entity: entity);

            await eventService.RaiseFolderRoleAddEventAsync(entity: result);

            return result;

        });

    public ValueTask DeleteFolderRoleAsync(FolderRole entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            await eventService.RaiseFolderRoleDeleteEventAsync(entity: entity);

            await processingService.DeleteFolderRoleAsync(entity: entity);

        });

    public ValueTask<IEnumerable<Result<FolderRole>>> AddOrUpdateFolderRole(IEnumerable<FolderRole> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.AddOrUpdateFolderRole(items: items);

        });

    public ValueTask DeleteAllFolderRoleAsync(IEnumerable<FolderRole> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.DeleteAllFolderRoleAsync(items: items);

        });
}