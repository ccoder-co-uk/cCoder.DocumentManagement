// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Orchestrations;

public interface IFolderOrchestrationService
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

    DMSResult GetFilesZippedAppPath(App app, IEnumerable<cCoder.DocumentManagement.Models.Path> paths);

    DMSResult GetAppPath(App app, cCoder.DocumentManagement.Models.Path path, string search = "");

    ValueTask UnpackAppPathAsync(App app, cCoder.DocumentManagement.Models.Path path, Stream content, bool ignoreArchiveRoot = false);

    ValueTask SaveAppPathAsync(App app, cCoder.DocumentManagement.Models.Path path);

    ValueTask DropAppPathAsync(App app, cCoder.DocumentManagement.Models.Path path);

    ValueTask CopyAppPathAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath);

    ValueTask MoveAppPathAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath);
}