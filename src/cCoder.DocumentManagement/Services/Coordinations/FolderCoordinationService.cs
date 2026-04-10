using cCoder.DocumentManagement.Services.Orchestrations;
using Folder = cCoder.Data.Models.DMS.Folder;

namespace cCoder.DocumentManagement.Services.Coordinations;

internal class FolderCoordinationService(
    IFolderOrchestrationService folderOrchestrationService,
    IFileOrchestrationService fileOrchestrationService) : IFolderCoordinationService
{
    public async ValueTask DeleteFolderAsync(Folder folder)
    {
        if (folder == null)
        {
            return;
        }

        Guid folderId = folder.Id;

        Guid[] childFileIds =
            [.. fileOrchestrationService.GetAll(ignoreFilters: true)
                .Where(file => file.FolderId == folderId)
                .Select(file => file.Id)];

        Guid[] childFolderIds =
            [.. folderOrchestrationService.GetAll(ignoreFilters: true)
                .Where(childFolder => childFolder.ParentId == folderId)
                .Select(childFolder => childFolder.Id)];

        foreach (Guid childFileId in childFileIds)
        {
            await fileOrchestrationService.DeleteAsync(childFileId);
        }

        foreach (Guid childFolderId in childFolderIds)
        {
            await folderOrchestrationService.DeleteAsync(childFolderId);
        }
    }
}
