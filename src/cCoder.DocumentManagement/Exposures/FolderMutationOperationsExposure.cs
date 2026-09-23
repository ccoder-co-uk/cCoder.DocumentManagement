// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Services.Aggregations;
using Folder = cCoder.Data.Models.DMS.Folder;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class FolderMutationOperationsExposure(
    IFolderMutationAggregationService folderMutationAggregationService)
    : IFolderMutationOperationsExposure
{
    public IQueryable<Folder> GetAllFolders(bool ignoreFilters = false) =>
        folderMutationAggregationService.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateFoldersAsync(IEnumerable<Folder> folders) =>
        folderMutationAggregationService.AddOrUpdateFolder(items: folders);

    public ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateAppFoldersAsync(IEnumerable<Folder> folders) =>
        folderMutationAggregationService.AddOrUpdateForAppFolderAsync(items: folders);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        folderMutationAggregationService.DeleteAllByAppIdAsync(appId: appId);
}