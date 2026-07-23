// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;


namespace cCoder.DocumentManagement.Services.Foundations;

public interface IFolderService
{
    Folder Get(Guid id);
    Folder GetWithRoles(Guid id, bool ignoreFilters = false);
    Folder GetForUpdate(Guid id, bool ignoreFilters = false);
    Folder GetByPath(int appId, string path, bool ignoreFilters = false);
    Folder GetByPathWithRoles(int appId, string path, bool ignoreFilters = false);
    Folder GetByPathWithParentAndRoles(int appId, string path, bool ignoreFilters = false);
    Folder GetByPathWithRolesAndFilesAndContents(int appId, string path, bool ignoreFilters = false);
    Folder GetByPathWithSubFoldersAndFiles(int appId, string path, bool ignoreFilters = false);
    IQueryable<Folder> GetAll(bool ignoreFilters = false);
    ValueTask<Folder> AddForPathBuildAsync(Folder folder);
    ValueTask<Folder> AddAsync(Folder folder);
    ValueTask<Folder> UpdateAsync(Folder folder);
    ValueTask<Folder> UpdateForAppAsync(Folder folder);
    ValueTask DeleteAsync(Guid id);
    ValueTask DeleteAllForAppAsync(IEnumerable<Folder> folders);
    ValueTask DeleteAllByAppIdAsync(int appId);
}