// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Exposures;

namespace cCoder.DocumentManagement.Services.Aggregations;

internal partial class AppAggregationService(IFolderMutationOperationsExposure folderOperationsExposure)
    : IAppAggregationService
{
    public ValueTask AddAppAsync(App newApp)
=>
        TryCatch(operation: async () =>
        {
            ValidateAppOnAdd(inputs: [newApp]);
            EnsureContentRootFolder(app: newApp);
            StampFoldersApp(app: newApp);

            _ = await folderOperationsExposure.AddOrUpdateAppFoldersAsync(folders: newApp.Folders ?? []);

        });

    public ValueTask UpdateAppAsync(App updatedApp)
=>
        TryCatch(operation: async () =>
        {
            ValidateAppOnUpdate(inputs: [updatedApp]);
            StampFoldersApp(app: updatedApp);

            _ = await folderOperationsExposure.AddOrUpdateFoldersAsync(folders: updatedApp.Folders ?? []);

        });

    public ValueTask DeleteAsync(int appId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId]);
            return folderOperationsExposure.DeleteAllByAppIdAsync(appId: appId);
        });

    private static void StampFoldersApp(App app)
    {
        foreach (Folder folder in app.Folders ?? [])
        {
            folder.AppId = app.Id;
        }
    }

    private static void EnsureContentRootFolder(App app)
    {
        if (app.Folders?.Any() == true)
        {
            return;
        }

        app.Folders =
        [
            new Folder
            {
                Name = "Content",
                Path = "Content"
            }
        ];
    }
}