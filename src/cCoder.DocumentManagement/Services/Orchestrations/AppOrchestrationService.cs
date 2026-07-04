using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Processings;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal class AppOrchestrationService(IFolderProcessingService folderProcessingService)
    : IAppOrchestrationService
{
    public async ValueTask AddAsync(App app)
    {
        StampFolders(app);
        _ = await folderProcessingService.AddOrUpdateForAppAsync(app.Folders ?? []);
    }

    public async ValueTask UpdateAsync(App app)
    {
        StampFolders(app);
        _ = await folderProcessingService.AddOrUpdateForAppAsync(app.Folders ?? []);
    }

    public ValueTask DeleteAsync(int appId) =>
        folderProcessingService.DeleteByAppIdAsync(appId);

    private static void StampFolders(App app)
    {
        foreach (Folder folder in app.Folders ?? [])
            folder.AppId = app.Id;
    }
}

