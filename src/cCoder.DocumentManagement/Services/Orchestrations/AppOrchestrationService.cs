using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal class AppOrchestrationService(IFolderOrchestrationService folderOrchestrationService)
    : IAppOrchestrationService
{
    public async ValueTask AddAsync(App app)
    {
        StampFolders(app);
        _ = await folderOrchestrationService.AddOrUpdate(app.Folders ?? []);
    }

    public async ValueTask UpdateAsync(App app)
    {
        StampFolders(app);
        _ = await folderOrchestrationService.AddOrUpdate(app.Folders ?? []);
    }

    public async ValueTask DeleteAsync(int appId)
    {
        Folder[] foldersToDelete =
            [.. folderOrchestrationService.GetAll(true)
                .Where(folder => folder.AppId == appId && folder.ParentId == null)];

        foreach (Folder folder in foldersToDelete)
        {
            await folderOrchestrationService.DeleteAsync(folder.Id);
        }
    }

    private static void StampFolders(App app)
    {
        foreach (Folder folder in app.Folders ?? [])
            folder.AppId = app.Id;
    }
}

