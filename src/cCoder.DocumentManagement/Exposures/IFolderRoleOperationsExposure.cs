// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Exposures;

public interface IFolderRoleOperationsExposure
{
    ValueTask<FolderRole> AddFolderRoleAsync(
        FolderRole newFolderRole);

    ValueTask DeleteFolderRoleAsync(
        FolderRole deletedFolderRole);
}