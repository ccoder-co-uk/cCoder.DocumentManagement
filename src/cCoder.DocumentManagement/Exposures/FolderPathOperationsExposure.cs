// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Aggregations;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class FolderPathOperationsExposure(
    FolderMutationAggregationService folderMutationAggregationService)
    : IFolderPathOperationsExposure
{
    public DMSResult GetFilesZippedAppPath(int appId, IEnumerable<string> paths) =>
        folderMutationAggregationService.GetFilesZippedAppPath(appId: appId, paths: paths);

    public DMSResult GetAppPath(int appId, string path, string search = "") =>
        folderMutationAggregationService.GetAppPath(appId: appId, path: path, search: search);

    public ValueTask UnpackAppPathAsync(int appId, string path, Stream content, bool ignoreArchiveRoot = false) =>
        folderMutationAggregationService.UnpackAppPathAsync(
            appId: appId,
            path: path,
            content: content,
            ignoreArchiveRoot: ignoreArchiveRoot);

    public ValueTask SaveAppPathAsync(int appId, string path) =>
        folderMutationAggregationService.SaveAppPathAsync(appId: appId, path: path);

    public ValueTask DropAppPathAsync(int appId, string path) =>
        folderMutationAggregationService.DropAppPathAsync(appId: appId, path: path);

    public ValueTask CopyAppPathAsync(int appId, string oldPath, string newPath) =>
        folderMutationAggregationService.CopyAppPathAsync(appId: appId, oldPath: oldPath, newPath: newPath);

    public ValueTask MoveAppPathAsync(int appId, string oldPath, string newPath) =>
        folderMutationAggregationService.MoveAppPathAsync(appId: appId, oldPath: oldPath, newPath: newPath);
}