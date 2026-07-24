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
    public ValueTask AddAppAsync(App newApp)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newApp]);
            StampFoldersApp(app: newApp);

            _ = await folderOrchestrationService.AddOrUpdateForAppFolderAsync(items: newApp.Folders ?? []);

        });

    public ValueTask UpdateAppAsync(App updatedApp)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedApp]);
            StampFoldersApp(app: updatedApp);

            _ = await folderOrchestrationService.AddOrUpdateForAppFolderAsync(items: updatedApp.Folders ?? []);

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