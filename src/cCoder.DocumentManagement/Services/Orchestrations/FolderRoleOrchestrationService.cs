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
            ValidateAllOnGet(inputs: [ignoreFilters]);
            return processingService.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<FolderRole> AddFolderRoleAsync(FolderRole newFolderRole)
=>
        TryCatch(operation: async () =>
        {
            ValidateFolderRoleOnAdd(inputs: [newFolderRole]);
            FolderRole result = await processingService.AddFolderRoleAsync(newFolderRole: newFolderRole);

            await eventService.RaiseFolderRoleAddEventAsync(folderRole: result);

            return result;

        });

    public ValueTask DeleteFolderRoleAsync(FolderRole deletedFolderRole)
=>
        TryCatch(operation: async () =>
        {
            ValidateFolderRoleOnDelete(inputs: [deletedFolderRole]);
            await eventService.RaiseFolderRoleDeleteEventAsync(folderRole: deletedFolderRole);

            await processingService.DeleteFolderRoleAsync(deletedFolderRole: deletedFolderRole);

        });

    public ValueTask<IEnumerable<Result<FolderRole>>> AddOrUpdateFolderRole(IEnumerable<FolderRole> items)
=>
        TryCatch(operation: () =>
        {
            ValidateOrUpdateFolderRoleOnAdd(inputs: [items]);
            return processingService.AddOrUpdateFolderRole(items: items);

        });

    public ValueTask DeleteAllFolderRoleAsync(IEnumerable<FolderRole> deletedFolderRole)
=>
        TryCatch(operation: () =>
        {
            ValidateAllFolderRoleOnDelete(inputs: [deletedFolderRole]);
            return processingService.DeleteAllFolderRoleAsync(deletedFolderRole: deletedFolderRole);

        });
}