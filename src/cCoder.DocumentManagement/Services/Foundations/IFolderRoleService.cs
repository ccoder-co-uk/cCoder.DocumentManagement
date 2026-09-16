// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;


namespace cCoder.DocumentManagement.Services.Foundations;

internal interface IFolderRoleService
{
    IQueryable<FolderRole> GetAll(bool ignoreFilters = false);
    ValueTask<FolderRole> AddFolderRoleAsync(FolderRole newFolderRole);
    ValueTask DeleteFolderRoleAsync(FolderRole deletedFolderRole);

    bool CanCreateFolderRole(FolderRole folderRole);
    bool CanDeleteFolderRole(FolderRole folderRole);
    bool FolderRoleExists(FolderRole folderRole);
}