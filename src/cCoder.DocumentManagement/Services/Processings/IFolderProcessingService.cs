// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Processings;

public interface IFolderProcessingService
{
    Folder Get(Guid folderId);

    IQueryable<Folder> GetAll(bool ignoreFilters = false);

    ValueTask<Folder> AddFolderAsync(Folder newFolder);

    ValueTask<Folder> UpdateFolderAsync(Folder updatedFolder);

    ValueTask DeleteAsync(Guid folderId);

    ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateFolder(IEnumerable<Folder> items);

    ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateForAppFolderAsync(IEnumerable<Folder> items);

    ValueTask DeleteAllFolderAsync(IEnumerable<Folder> deletedFolder);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<List<Result<Guid?>>> CopyAsync(string source, string destination, int sourceAppId, int destAppId);

    ValueTask HandleFolderDeleteEventAsync(Folder folder);

    DMSResult GetFilesZippedAppPath(int appId, IEnumerable<cCoder.DocumentManagement.Dependencies.Path> paths);

    DMSResult GetAppPath(int appId, cCoder.DocumentManagement.Dependencies.Path path, string search = "");

    ValueTask UnpackAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path, Stream content, bool ignoreArchiveRoot = false);

    ValueTask SaveAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path);

    ValueTask DropAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path);

    ValueTask CopyAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath);

    ValueTask MoveAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath);
}