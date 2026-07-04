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
        _ = await folderOrchestrationService.AddOrUpdateForAppAsync(app.Folders ?? []);
    }

    public async ValueTask UpdateAsync(App app)
    {
        StampFolders(app);
        _ = await folderOrchestrationService.AddOrUpdateForAppAsync(app.Folders ?? []);
    }

    public ValueTask DeleteAsync(int appId) =>
        folderOrchestrationService.DeleteAllByAppIdAsync(appId);

    private static void StampFolders(App app)
    {
        foreach (Folder folder in app.Folders ?? [])
            folder.AppId = app.Id;
    }
}

