// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal partial class AppOrchestrationService(IFolderOrchestrationService folderOrchestrationService)
    : IAppOrchestrationService
{
    public ValueTask AddAppAsync(App app)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [app]);
            StampFoldersApp(app: app);

            _ = await folderOrchestrationService.AddOrUpdateForAppFolderAsync(items: app.Folders ?? []);

        });

    public ValueTask UpdateAppAsync(App app)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [app]);
            StampFoldersApp(app: app);

            _ = await folderOrchestrationService.AddOrUpdateForAppFolderAsync(items: app.Folders ?? []);

        });

    public ValueTask DeleteAsync(int appId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId]);
            return folderOrchestrationService.DeleteAllByAppIdAsync(appId: appId);
        });

    private static void StampFoldersApp(App app)
    {
        foreach (Folder folder in app.Folders ?? [])
        {
            folder.AppId = app.Id;
        }
    }
}