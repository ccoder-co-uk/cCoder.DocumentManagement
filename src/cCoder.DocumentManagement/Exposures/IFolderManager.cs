// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Exposures;

public interface IFolderManager
{
    Folder Get(Guid folderId);

    IQueryable<Folder> GetAll(bool ignoreFilters = false);

    ValueTask<Folder> AddFolderAsync(Folder newFolder);

    ValueTask<Folder> UpdateFolderAsync(Folder updatedFolder);

    ValueTask DeleteAsync(Guid folderId);
    ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateFolder(IEnumerable<Folder> items);

    ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateForAppFolderAsync(IEnumerable<Folder> items);

    ValueTask DeleteAllFolderAsync(IEnumerable<Folder> deletedFolder);

    ValueTask DeleteAllByAppIdAsync(int appId);

    ValueTask<List<Result<Guid?>>> CopyAsync(string source, string destination, int sourceAppId, int destAppId);

    ValueTask HandleFolderDeleteEventAsync(Folder folder);

}