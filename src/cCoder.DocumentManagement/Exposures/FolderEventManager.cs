// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Services.Coordinations;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class FolderEventManager(
    IFolderCoordinationService folderCoordinationService)
    : IFolderEventManager
{
    public ValueTask HandleFolderDeleteEventAsync(Folder folder) =>
        folderCoordinationService.DeleteFolderAsync(
            deletedFolder: folder);
}