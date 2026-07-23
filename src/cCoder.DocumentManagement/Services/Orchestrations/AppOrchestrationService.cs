// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        StampFolders(app: app);
        _ = await folderOrchestrationService.AddOrUpdateForAppAsync(items: app.Folders ?? []);
    }

    public async ValueTask UpdateAsync(App app)
    {
        StampFolders(app: app);
        _ = await folderOrchestrationService.AddOrUpdateForAppAsync(items: app.Folders ?? []);
    }

    public ValueTask DeleteAsync(int appId) =>
        folderOrchestrationService.DeleteAllByAppIdAsync(appId: appId);

    private static void StampFolders(App app)
    {
        foreach (Folder folder in app.Folders ?? [])
        {
            folder.AppId = app.Id;
        }
    }
}