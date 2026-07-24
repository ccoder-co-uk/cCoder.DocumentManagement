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
    Folder Get(Guid id);

    IQueryable<Folder> GetAll(bool ignoreFilters = false);

    ValueTask<Folder> AddFolderAsync(Folder entity);

    ValueTask<Folder> UpdateFolderAsync(Folder entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateFolder(IEnumerable<Folder> items);

    ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateForAppFolderAsync(IEnumerable<Folder> items);

    ValueTask DeleteAllFolderAsync(IEnumerable<Folder> items);

    ValueTask DeleteByAppIdAsync(int appId);

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