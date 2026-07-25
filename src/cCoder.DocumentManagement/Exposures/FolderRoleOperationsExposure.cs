// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class FolderRoleOperationsExposure(
    IFolderRoleService folderRoleService)
    : IFolderRoleOperationsExposure
{
    public ValueTask<FolderRole> AddFolderRoleAsync(
        FolderRole newFolderRole) =>
        folderRoleService.AddFolderRoleAsync(
            newFolderRole: newFolderRole);

    public ValueTask DeleteFolderRoleAsync(
        FolderRole deletedFolderRole) =>
        folderRoleService.DeleteFolderRoleAsync(
            deletedFolderRole: deletedFolderRole);
}