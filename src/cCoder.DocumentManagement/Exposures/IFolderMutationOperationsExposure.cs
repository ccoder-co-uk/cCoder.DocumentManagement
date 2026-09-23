// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using Folder = cCoder.Data.Models.DMS.Folder;

namespace cCoder.DocumentManagement.Exposures;

internal interface IFolderMutationOperationsExposure
{
    IQueryable<Folder> GetAllFolders(bool ignoreFilters = false);
    ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateFoldersAsync(IEnumerable<Folder> folders);
    ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateAppFoldersAsync(IEnumerable<Folder> folders);
    ValueTask DeleteAllByAppIdAsync(int appId);
}