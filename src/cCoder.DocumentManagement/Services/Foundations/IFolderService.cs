// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.DMS;


namespace cCoder.DocumentManagement.Services.Foundations;

internal interface IFolderService
{
    Folder Get(Guid folderId);
    Folder GetWithRoles(Guid folderId, bool ignoreFilters = false);
    Folder GetForUpdate(Guid folderId, bool ignoreFilters = false);
    Folder GetByPath(int appId, string path, bool ignoreFilters = false);
    Folder GetByPathWithRoles(int appId, string path, bool ignoreFilters = false);
    Folder GetByPathWithParentAndRoles(int appId, string path, bool ignoreFilters = false);
    Folder GetByPathWithRolesAndFilesAndContents(int appId, string path, bool ignoreFilters = false);
    Folder GetFolderByPathWithSubFoldersAndFiles(int appId, string path, bool ignoreFilters = false);
    IQueryable<Folder> GetAll(bool ignoreFilters = false);
    ValueTask<Folder> AddForPathBuildFolderAsync(Folder newFolder);
    ValueTask<Folder> AddFolderAsync(Folder newFolder);
    ValueTask<Folder> UpdateFolderAsync(Folder updatedFolder);
    ValueTask<Folder> UpdateForAppFolderAsync(Folder updatedFolder);
    ValueTask DeleteAsync(Guid folderId);
    ValueTask DeleteAllForAppFolderAsync(IEnumerable<Folder> deletedFolder);
    ValueTask DeleteAllByAppIdAsync(int appId);
}