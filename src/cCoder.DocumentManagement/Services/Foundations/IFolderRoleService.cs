// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;


namespace cCoder.DocumentManagement.Services.Foundations;

public interface IFolderRoleService
{
    IQueryable<FolderRole> GetAll(bool ignoreFilters = false);
    ValueTask<FolderRole> AddFolderRoleAsync(FolderRole folderRole);
    ValueTask DeleteFolderRoleAsync(FolderRole folderRole);
}