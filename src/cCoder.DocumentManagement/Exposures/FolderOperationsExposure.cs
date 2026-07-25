// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Services.Foundations;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class FolderOperationsExposure(
    IFolderService folderService)
    : IFolderOperationsExposure
{
    public IQueryable<Folder> GetAllFolders(bool ignoreFilters = false) =>
        folderService.GetAll(
            ignoreFilters: ignoreFilters);

    public Folder GetFolderWithRoles(
        Guid folderId,
        bool ignoreFilters = false) =>
        folderService.GetWithRoles(
            folderId: folderId,
            ignoreFilters: ignoreFilters);

    public Folder GetFolderByPath(
        int appId,
        string path,
        bool ignoreFilters = false) =>
        folderService.GetByPath(
            appId: appId,
            path: path,
            ignoreFilters: ignoreFilters);

    public Folder GetFolderByPathWithRoles(
        int appId,
        string path,
        bool ignoreFilters = false) =>
        folderService.GetByPathWithRoles(
            appId: appId,
            path: path,
            ignoreFilters: ignoreFilters);

    public ValueTask<Folder> AddFolderAsync(Folder newFolder) =>
        folderService.AddFolderAsync(
            newFolder: newFolder);
}