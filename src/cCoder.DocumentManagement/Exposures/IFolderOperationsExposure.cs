// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Exposures;

public interface IFolderOperationsExposure
{
    IQueryable<Folder> GetAllFolders(bool ignoreFilters = false);

    Folder GetFolderWithRoles(
        Guid folderId,
        bool ignoreFilters = false);

    Folder GetFolderByPath(
        int appId,
        string path,
        bool ignoreFilters = false);

    Folder GetFolderByPathWithRoles(
        int appId,
        string path,
        bool ignoreFilters = false);

    ValueTask<Folder> AddFolderAsync(Folder newFolder);
}