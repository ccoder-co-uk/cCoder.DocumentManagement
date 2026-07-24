// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Orchestrations;
using Folder = cCoder.Data.Models.DMS.Folder;

namespace cCoder.DocumentManagement.Services.Coordinations;

internal partial class FolderCoordinationService(
    IFolderOrchestrationService folderOrchestrationService,
    IFileOrchestrationService fileOrchestrationService) : IFolderCoordinationService
{
    public ValueTask DeleteFolderAsync(Folder deletedFolder)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [deletedFolder]);
            if (deletedFolder == null)
            {
                return;
            }


            Guid folderId = deletedFolder.Id;


            Guid[] childFileIds =
                [.. fileOrchestrationService.GetAll(ignoreFilters: true)
                .Where(predicate:file => file.FolderId == folderId)
                .Select(selector:file => file.Id)];


            Guid[] childFolderIds =
                [.. folderOrchestrationService.GetAll(ignoreFilters: true)
                .Where(predicate:childFolder => childFolder.ParentId == folderId)
                .Select(selector:childFolder => childFolder.Id)];


            foreach (Guid childFileId in childFileIds)
            {
                await fileOrchestrationService.DeleteAsync(fileId: childFileId);
            }


            foreach (Guid childFolderId in childFolderIds)
            {
                await folderOrchestrationService.DeleteAsync(folderId: childFolderId);
            }

        });
}